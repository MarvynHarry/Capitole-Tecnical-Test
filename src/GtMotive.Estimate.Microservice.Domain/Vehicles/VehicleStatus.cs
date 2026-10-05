namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Rental status of a vehicle.
    /// </summary>
    public enum VehicleStatus
    {
        /// <summary>
        /// The vehicle is in the fleet and can be rented.
        /// </summary>
        Available = 0,

        /// <summary>
        /// The vehicle is currently rented by someone.
        /// </summary>
        Rented = 1,
    }
}
