using System.Collections.Generic;
using System.Threading.Tasks;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Port (secondary / driven side) to persist <see cref="Vehicle"/> aggregates.
    /// </summary>
    public interface IVehicleRepository
    {
        /// <summary>
        /// Adds a new vehicle.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        /// <returns>Task.</returns>
        Task Add(Vehicle vehicle);

        /// <summary>
        /// Persists the changes of an existing vehicle.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        /// <returns>Task.</returns>
        Task Update(Vehicle vehicle);

        /// <summary>
        /// Gets a vehicle by its identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <returns>The vehicle, or null if it does not exist.</returns>
        Task<Vehicle> GetById(VehicleId id);

        /// <summary>
        /// Gets the vehicles that are not rented.
        /// </summary>
        /// <returns>Vehicles with <see cref="VehicleStatus.Available"/> status.</returns>
        Task<IReadOnlyCollection<Vehicle>> GetAvailable();

        /// <summary>
        /// Checks whether a vehicle with the given license plate exists.
        /// </summary>
        /// <param name="licensePlate">License plate.</param>
        /// <returns>True if it exists.</returns>
        Task<bool> ExistsWithLicensePlate(LicensePlate licensePlate);

        /// <summary>
        /// Checks whether the person has a vehicle rented right now.
        /// </summary>
        /// <param name="renter">Person.</param>
        /// <returns>True if the person is currently renting a vehicle.</returns>
        Task<bool> HasActiveRental(RenterId renter);
    }
}
