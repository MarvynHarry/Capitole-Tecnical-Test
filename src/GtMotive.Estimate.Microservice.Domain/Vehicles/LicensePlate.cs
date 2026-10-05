using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Value object that represents a vehicle license plate. It is stored normalized
    /// (upper case, without blanks or hyphens) so "1234-abc" and "1234 ABC" are the same plate.
    /// </summary>
    public readonly record struct LicensePlate
    {
        private const int MaxLength = 15;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensePlate"/> class.
        /// </summary>
        /// <param name="value">Raw license plate.</param>
        public LicensePlate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException("The license plate is required.");
            }

            var normalized = value
                .Replace(" ", string.Empty, StringComparison.Ordinal)
                .Replace("-", string.Empty, StringComparison.Ordinal)
                .ToUpperInvariant();

            if (normalized.Length > MaxLength)
            {
                throw new DomainException($"The license plate can not be longer than {MaxLength} characters.");
            }

            Value = normalized;
        }

        /// <summary>
        /// Gets the normalized license plate.
        /// </summary>
        public string Value { get; }

        /// <inheritdoc />
        public override string ToString() => Value;
    }
}
