using System.Collections.Generic;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetAvailableVehicles
{
    /// <summary>
    /// Output message of the Get Available Vehicles use case.
    /// </summary>
    /// <param name="Vehicles">Vehicles that can be rented.</param>
    public sealed record GetAvailableVehiclesOutput(IReadOnlyCollection<VehicleOutput> Vehicles) : IUseCaseOutput;
}
