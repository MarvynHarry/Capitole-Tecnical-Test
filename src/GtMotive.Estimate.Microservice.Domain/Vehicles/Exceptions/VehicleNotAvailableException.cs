using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions
{
    /// <summary>
    /// Thrown when the vehicle can not be rented because it is already rented.
    /// </summary>
    public sealed class VehicleNotAvailableException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotAvailableException"/> class.
        /// </summary>
        public VehicleNotAvailableException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotAvailableException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public VehicleNotAvailableException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotAvailableException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public VehicleNotAvailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
