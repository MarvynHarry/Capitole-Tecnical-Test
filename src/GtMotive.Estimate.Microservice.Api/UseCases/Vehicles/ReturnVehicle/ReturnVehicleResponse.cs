using System;
using System.ComponentModel.DataAnnotations;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.ReturnVehicle;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.ReturnVehicle
{
    /// <summary>
    /// View model of a finished rental.
    /// </summary>
    public sealed class ReturnVehicleResponse
    {
        public ReturnVehicleResponse(ReturnVehicleOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);

            Vehicle = new VehicleResponse(output.Vehicle);
            RenterId = output.RenterId;
            RentedAt = output.RentedAt;
            ReturnedAt = output.ReturnedAt;
        }

        /// <summary>
        /// Gets the returned vehicle, available again.
        /// </summary>
        [Required]
        public VehicleResponse Vehicle { get; }

        /// <summary>
        /// Gets the person that had the vehicle rented.
        /// </summary>
        [Required]
        public string RenterId { get; }

        /// <summary>
        /// Gets the start (UTC) of the finished rental.
        /// </summary>
        [Required]
        public DateTime RentedAt { get; }

        /// <summary>
        /// Gets the moment (UTC) of the return.
        /// </summary>
        [Required]
        public DateTime ReturnedAt { get; }
    }
}
