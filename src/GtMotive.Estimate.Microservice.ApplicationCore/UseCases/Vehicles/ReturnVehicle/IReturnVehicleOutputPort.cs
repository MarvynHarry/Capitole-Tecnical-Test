namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.ReturnVehicle
{
    /// <summary>
    /// Output port of the Return Vehicle use case.
    /// </summary>
    public interface IReturnVehicleOutputPort : IOutputPortStandard<ReturnVehicleOutput>, IOutputPortNotFound
    {
        /// <summary>
        /// Informs that the vehicle is not rented, so it can not be returned.
        /// </summary>
        /// <param name="message">Text description.</param>
        void VehicleNotRentedHandle(string message);
    }
}
