namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Value object that identifies the person that rents a vehicle (e.g. national identity document).
    /// </summary>
    public readonly record struct RenterId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenterId"/> class.
        /// </summary>
        /// <param name="value">Identity document of the person.</param>
        public RenterId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException("The renter identifier is required.");
            }

            Value = value.Trim().ToUpperInvariant();
        }

        /// <summary>
        /// Gets the normalized identifier.
        /// </summary>
        public string Value { get; }

        /// <inheritdoc />
        public override string ToString() => Value;
    }
}
