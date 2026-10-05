using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles
{
    /// <summary>
    /// Snapshot of a vehicle returned by the vehicle use cases.
    /// </summary>
    public sealed class VehicleOutput
    {
        private VehicleOutput(Vehicle vehicle)
        {
            Id = vehicle.Id.Value;
            Brand = vehicle.Brand;
            Model = vehicle.Model;
            LicensePlate = vehicle.LicensePlate.Value;
            ManufactureDate = vehicle.ManufactureDate;
            Status = vehicle.Status;
            RentedBy = vehicle.CurrentRental?.Renter.Value;
            RentedAt = vehicle.CurrentRental?.StartedAt;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        public string Brand { get; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// Gets the license plate.
        /// </summary>
        public string LicensePlate { get; }

        /// <summary>
        /// Gets the manufacture date.
        /// </summary>
        public DateTime ManufactureDate { get; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public VehicleStatus Status { get; }

        /// <summary>
        /// Gets the current renter, if any.
        /// </summary>
        public string RentedBy { get; }

        /// <summary>
        /// Gets the start of the current rental, if any.
        /// </summary>
        public DateTime? RentedAt { get; }

        /// <summary>
        /// Builds the output from a vehicle aggregate.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        /// <returns>The output snapshot.</returns>
        public static VehicleOutput From(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            return new VehicleOutput(vehicle);
        }
    }
}
