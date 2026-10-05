using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.RentVehicle
{
    /// <summary>
    /// Input message of the Rent Vehicle use case.
    /// </summary>
    /// <param name="VehicleId">Vehicle to rent.</param>
    /// <param name="RenterId">Person that rents the vehicle.</param>
    public sealed record RentVehicleInput(VehicleId VehicleId, RenterId RenterId) : IUseCaseInput;
}
