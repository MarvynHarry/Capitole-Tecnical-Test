using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Value object that describes an active rental of a vehicle.
    /// </summary>
    /// <param name="Renter">Person that rents the vehicle.</param>
    /// <param name="StartedAt">Moment (UTC) in which the rental started.</param>
    public sealed record Rental(RenterId Renter, DateTime StartedAt);
}
