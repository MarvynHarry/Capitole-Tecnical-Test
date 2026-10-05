namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.RentVehicle
{
    /// <summary>
    /// Output port of the Rent Vehicle use case.
    /// </summary>
    public interface IRentVehicleOutputPort : IOutputPortStandard<RentVehicleOutput>, IOutputPortNotFound
    {
        /// <summary>
        /// Informs that the vehicle is already rented.
        /// </summary>
        /// <param name="message">Text description.</param>
        void VehicleNotAvailableHandle(string message);

        /// <summary>
        /// Informs that the person already has another vehicle rented.
        /// </summary>
        /// <param name="message">Text description.</param>
        void RenterAlreadyHasVehicleHandle(string message);
    }
}
