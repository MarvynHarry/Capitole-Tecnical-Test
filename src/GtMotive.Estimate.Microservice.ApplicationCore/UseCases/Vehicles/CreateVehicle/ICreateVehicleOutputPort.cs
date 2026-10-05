namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle
{
    /// <summary>
    /// Output port of the Create Vehicle use case.
    /// </summary>
    public interface ICreateVehicleOutputPort : IOutputPortStandard<CreateVehicleOutput>
    {
        /// <summary>
        /// Informs that the fleet already contains a vehicle with the same license plate.
        /// </summary>
        /// <param name="message">Text description.</param>
        void LicensePlateAlreadyExistsHandle(string message);
    }
}
