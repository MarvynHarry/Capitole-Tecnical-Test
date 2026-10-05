using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions
{
    /// <summary>
    /// Thrown when a person tries to rent a vehicle while already renting another one.
    /// </summary>
    public sealed class RenterAlreadyHasVehicleException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenterAlreadyHasVehicleException"/> class.
        /// </summary>
        public RenterAlreadyHasVehicleException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenterAlreadyHasVehicleException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public RenterAlreadyHasVehicleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenterAlreadyHasVehicleException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public RenterAlreadyHasVehicleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
