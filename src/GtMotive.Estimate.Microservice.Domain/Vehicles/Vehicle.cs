using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Aggregate root that represents a vehicle of the renting fleet.
    /// </summary>
    public sealed class Vehicle
    {
        /// <summary>
        /// Maximum age, in years, allowed for a vehicle to be part of the fleet.
        /// </summary>
        public const int MaxAgeInYears = 5;

        private Vehicle(
            VehicleId id,
            string brand,
            string model,
            LicensePlate licensePlate,
            DateTime manufactureDate,
            Rental currentRental)
        {
            Id = id;
            Brand = brand;
            Model = model;
            LicensePlate = licensePlate;
            ManufactureDate = manufactureDate;
            CurrentRental = currentRental;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        public VehicleId Id { get; }

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
        public LicensePlate LicensePlate { get; }

        /// <summary>
        /// Gets the manufacture date (date part only).
        /// </summary>
        public DateTime ManufactureDate { get; }

        /// <summary>
        /// Gets the active rental, or null when the vehicle is not rented.
        /// </summary>
        public Rental CurrentRental { get; private set; }

        /// <summary>
        /// Gets the current status.
        /// </summary>
        public VehicleStatus Status => CurrentRental is null ? VehicleStatus.Available : VehicleStatus.Rented;

        /// <summary>
        /// Gets a value indicating whether the vehicle can be rented right now.
        /// </summary>
        public bool IsAvailable => Status == VehicleStatus.Available;

        /// <summary>
        /// Creates a brand new vehicle for the fleet.
        /// </summary>
        /// <param name="brand">Brand.</param>
        /// <param name="model">Model.</param>
        /// <param name="licensePlate">License plate.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        /// <param name="today">Current date, used to validate the age of the vehicle.</param>
        /// <returns>The new vehicle, available to be rented.</returns>
        public static Vehicle Create(
            string brand,
            string model,
            LicensePlate licensePlate,
            DateTime manufactureDate,
            DateTime today)
        {
            if (string.IsNullOrWhiteSpace(brand))
            {
                throw new DomainException("The vehicle brand is required.");
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                throw new DomainException("The vehicle model is required.");
            }

            if (manufactureDate.Date > today.Date)
            {
                throw new DomainException("The manufacture date can not be in the future.");
            }

            if (!IsWithinMaxAge(manufactureDate, today))
            {
                throw new VehicleTooOldException(
                    $"The vehicle can not be added to the fleet: its manufacture date ({manufactureDate:yyyy-MM-dd}) is older than {MaxAgeInYears} years.");
            }

            return new Vehicle(
                VehicleId.New(),
                brand.Trim(),
                model.Trim(),
                licensePlate,
                manufactureDate.Date,
                null);
        }

        /// <summary>
        /// Rebuilds a vehicle from its persisted state. Only meant to be used by repositories.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="brand">Brand.</param>
        /// <param name="model">Model.</param>
        /// <param name="licensePlate">License plate.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        /// <param name="currentRental">Active rental, or null if the vehicle is not rented.</param>
        /// <returns>The rebuilt vehicle.</returns>
        public static Vehicle Restore(
            VehicleId id,
            string brand,
            string model,
            LicensePlate licensePlate,
            DateTime manufactureDate,
            Rental currentRental)
        {
            return new Vehicle(id, brand, model, licensePlate, manufactureDate, currentRental);
        }

        /// <summary>
        /// Checks whether the vehicle still fulfils the fleet age restriction.
        /// </summary>
        /// <param name="today">Current date.</param>
        /// <returns>True if the vehicle is not older than <see cref="MaxAgeInYears"/> years.</returns>
        public bool IsEligibleForFleet(DateTime today)
        {
            return IsWithinMaxAge(ManufactureDate, today);
        }

        /// <summary>
        /// Rents the vehicle to a person.
        /// </summary>
        /// <param name="renter">The person that rents the vehicle.</param>
        /// <param name="now">Current moment (UTC).</param>
        public void Rent(RenterId renter, DateTime now)
        {
            if (!IsAvailable)
            {
                throw new VehicleNotAvailableException($"The vehicle {Id} is already rented.");
            }

            if (!IsEligibleForFleet(now))
            {
                throw new VehicleTooOldException($"The vehicle {Id} is older than {MaxAgeInYears} years and can not be rented.");
            }

            CurrentRental = new Rental(renter, now);
        }

        /// <summary>
        /// Returns the vehicle to the fleet, making it available again.
        /// </summary>
        /// <returns>The rental that has just finished.</returns>
        public Rental Return()
        {
            if (CurrentRental is null)
            {
                throw new VehicleNotRentedException($"The vehicle {Id} is not rented.");
            }

            var finishedRental = CurrentRental;
            CurrentRental = null;

            return finishedRental;
        }

        private static bool IsWithinMaxAge(DateTime manufactureDate, DateTime today)
        {
            return manufactureDate.Date >= today.Date.AddYears(-MaxAgeInYears);
        }
    }
}
