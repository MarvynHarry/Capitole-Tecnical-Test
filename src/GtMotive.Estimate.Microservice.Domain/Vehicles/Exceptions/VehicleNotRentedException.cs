using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions
{
    /// <summary>
    /// Thrown when the vehicle can not be returned because it is not rented.
    /// </summary>
    public sealed class VehicleNotRentedException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotRentedException"/> class.
        /// </summary>
        public VehicleNotRentedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotRentedException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public VehicleNotRentedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotRentedException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public VehicleNotRentedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
