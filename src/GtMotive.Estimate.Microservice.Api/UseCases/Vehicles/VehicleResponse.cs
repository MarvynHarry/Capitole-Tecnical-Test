using System;
using System.ComponentModel.DataAnnotations;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    /// <summary>
    /// View model of a vehicle.
    /// </summary>
    public sealed class VehicleResponse
    {
        public VehicleResponse(VehicleOutput vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            Id = vehicle.Id;
            Brand = vehicle.Brand;
            Model = vehicle.Model;
            LicensePlate = vehicle.LicensePlate;
            ManufactureDate = vehicle.ManufactureDate;
            Status = vehicle.Status.ToString();
            RentedBy = vehicle.RentedBy;
            RentedAt = vehicle.RentedAt;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        [Required]
        public Guid Id { get; }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        [Required]
        public string Brand { get; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        [Required]
        public string Model { get; }

        /// <summary>
        /// Gets the license plate (normalized).
        /// </summary>
        [Required]
        public string LicensePlate { get; }

        /// <summary>
        /// Gets the manufacture date.
        /// </summary>
        [Required]
        public DateTime ManufactureDate { get; }

        /// <summary>
        /// Gets the status: Available or Rented.
        /// </summary>
        [Required]
        public string Status { get; }

        /// <summary>
        /// Gets the person that has the vehicle rented, if any.
        /// </summary>
        public string RentedBy { get; }

        /// <summary>
        /// Gets the start (UTC) of the current rental, if any.
        /// </summary>
        public DateTime? RentedAt { get; }
    }
}
