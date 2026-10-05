namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.RentVehicle
{
    /// <summary>
    /// Output message of the Rent Vehicle use case.
    /// </summary>
    /// <param name="Vehicle">The rented vehicle.</param>
    public sealed record RentVehicleOutput(VehicleOutput Vehicle) : IUseCaseOutput;
}
