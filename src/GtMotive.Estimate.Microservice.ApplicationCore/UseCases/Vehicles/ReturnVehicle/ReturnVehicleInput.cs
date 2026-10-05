using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.ReturnVehicle
{
    /// <summary>
    /// Input message of the Return Vehicle use case.
    /// </summary>
    /// <param name="VehicleId">Vehicle to return.</param>
    public sealed record ReturnVehicleInput(VehicleId VehicleId) : IUseCaseInput;
}
