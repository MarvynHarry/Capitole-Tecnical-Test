using System;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.ReturnVehicle
{
    /// <summary>
    /// Output message of the Return Vehicle use case.
    /// </summary>
    /// <param name="Vehicle">The returned vehicle (available again).</param>
    /// <param name="RenterId">Person that had the vehicle rented.</param>
    /// <param name="RentedAt">Start of the finished rental.</param>
    /// <param name="ReturnedAt">Moment of the return.</param>
    public sealed record ReturnVehicleOutput(
        VehicleOutput Vehicle,
        string RenterId,
        DateTime RentedAt,
        DateTime ReturnedAt) : IUseCaseOutput;
}
