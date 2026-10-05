namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle
{
    /// <summary>
    /// Output message of the Create Vehicle use case.
    /// </summary>
    /// <param name="Vehicle">The created vehicle.</param>
    public sealed record CreateVehicleOutput(VehicleOutput Vehicle) : IUseCaseOutput;
}
