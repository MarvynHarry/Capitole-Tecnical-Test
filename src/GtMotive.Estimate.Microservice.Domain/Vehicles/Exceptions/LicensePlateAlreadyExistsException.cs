using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions
{
    /// <summary>
    /// Thrown when the fleet already contains a vehicle with the same license plate.
    /// </summary>
    public sealed class LicensePlateAlreadyExistsException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LicensePlateAlreadyExistsException"/> class.
        /// </summary>
        public LicensePlateAlreadyExistsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensePlateAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public LicensePlateAlreadyExistsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensePlateAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public LicensePlateAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
