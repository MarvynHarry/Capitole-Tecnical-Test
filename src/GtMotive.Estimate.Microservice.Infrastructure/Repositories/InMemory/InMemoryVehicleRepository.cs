using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories.InMemory
{
    /// <summary>
    /// In-memory adapter of <see cref="IVehicleRepository"/>. It is the default persistence, so the
    /// microservice can run (and be tested) without any external dependency. Must be registered as singleton.
    /// </summary>
    public sealed class InMemoryVehicleRepository : IVehicleRepository
    {
        private readonly Dictionary<Guid, VehicleRecord> _vehicles = [];
        private readonly object _sync = new();

        public Task Add(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            lock (_sync)
            {
                if (_vehicles.Values.Any(v => v.LicensePlate == vehicle.LicensePlate.Value))
                {
                    throw new LicensePlateAlreadyExistsException($"A vehicle with license plate {vehicle.LicensePlate} already exists in the fleet.");
                }

                _vehicles.Add(vehicle.Id.Value, VehicleRecord.From(vehicle));
            }

            return Task.CompletedTask;
        }

        public Task Update(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            lock (_sync)
            {
                if (!_vehicles.ContainsKey(vehicle.Id.Value))
                {
                    throw new InvalidOperationException($"The vehicle {vehicle.Id} does not exist.");
                }

                // Same guarantee that the unique index gives in MongoDB: one active rental per person.
                var renter = vehicle.CurrentRental?.Renter.Value;
                if (renter is not null && _vehicles.Values.Any(v => v.RentedBy == renter && v.Id != vehicle.Id.Value))
                {
                    throw new RenterAlreadyHasVehicleException($"The person {renter} already has a vehicle rented.");
                }

                _vehicles[vehicle.Id.Value] = VehicleRecord.From(vehicle);
            }

            return Task.CompletedTask;
        }

        public Task<Vehicle> GetById(VehicleId id)
        {
            lock (_sync)
            {
                return Task.FromResult(_vehicles.TryGetValue(id.Value, out var record) ? record.ToVehicle() : null);
            }
        }

        public Task<IReadOnlyCollection<Vehicle>> GetAvailable()
        {
            lock (_sync)
            {
                IReadOnlyCollection<Vehicle> available = _vehicles.Values
                    .Where(v => v.RentedBy is null)
                    .Select(v => v.ToVehicle())
                    .ToList();

                return Task.FromResult(available);
            }
        }

        public Task<bool> ExistsWithLicensePlate(LicensePlate licensePlate)
        {
            lock (_sync)
            {
                return Task.FromResult(_vehicles.Values.Any(v => v.LicensePlate == licensePlate.Value));
            }
        }

        public Task<bool> HasActiveRental(RenterId renter)
        {
            lock (_sync)
            {
                return Task.FromResult(_vehicles.Values.Any(v => v.RentedBy == renter.Value));
            }
        }

        /// <summary>
        /// Immutable copy of the vehicle state, so changes made to an aggregate are not visible
        /// until <see cref="Update"/> is called (as it happens with a real database).
        /// </summary>
        private sealed record VehicleRecord(
            Guid Id,
            string Brand,
            string Model,
            string LicensePlate,
            DateTime ManufactureDate,
            string RentedBy,
            DateTime? RentedAt)
        {
            public static VehicleRecord From(Vehicle vehicle) => new(
                vehicle.Id.Value,
                vehicle.Brand,
                vehicle.Model,
                vehicle.LicensePlate.Value,
                vehicle.ManufactureDate,
                vehicle.CurrentRental?.Renter.Value,
                vehicle.CurrentRental?.StartedAt);

            public Vehicle ToVehicle() => Vehicle.Restore(
                new VehicleId(Id),
                Brand,
                Model,
                new LicensePlate(LicensePlate),
                ManufactureDate,
                RentedBy is null ? null : new Rental(new RenterId(RentedBy), RentedAt.GetValueOrDefault()));
        }
    }
}
