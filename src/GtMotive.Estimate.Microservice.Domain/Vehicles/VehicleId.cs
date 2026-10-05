using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Value object that identifies a vehicle.
    /// </summary>
    public readonly record struct VehicleId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleId"/> class.
        /// </summary>
        /// <param name="value">Identifier value.</param>
        public VehicleId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new DomainException("The vehicle identifier can not be empty.");
            }

            Value = value;
        }

        /// <summary>
        /// Gets the identifier value.
        /// </summary>
        public Guid Value { get; }

        /// <summary>
        /// Generates a new unique identifier.
        /// </summary>
        /// <returns>A new <see cref="VehicleId"/>.</returns>
        public static VehicleId New() => new(Guid.NewGuid());

        /// <inheritdoc />
        public override string ToString() => Value.ToString();
    }
}
