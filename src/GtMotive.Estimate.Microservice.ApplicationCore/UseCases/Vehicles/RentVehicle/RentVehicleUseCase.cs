using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.RentVehicle
{
    /// <summary>
    /// Rents a vehicle to a person. A person can only have one vehicle rented at the same time.
    /// </summary>
    /// <param name="vehicleRepository">Vehicle repository.</param>
    /// <param name="outputPort">Output port.</param>
    /// <param name="timeProvider">Clock.</param>
    public sealed class RentVehicleUseCase(
        IVehicleRepository vehicleRepository,
        IRentVehicleOutputPort outputPort,
        TimeProvider timeProvider) : IUseCase<RentVehicleInput>
    {
        /// <inheritdoc />
        public async Task Execute(RentVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var vehicle = await vehicleRepository.GetById(input.VehicleId);

            if (vehicle is null)
            {
                outputPort.NotFoundHandle($"The vehicle {input.VehicleId} does not exist.");
                return;
            }

            if (!vehicle.IsAvailable)
            {
                outputPort.VehicleNotAvailableHandle($"The vehicle {input.VehicleId} is already rented.");
                return;
            }

            if (await vehicleRepository.HasActiveRental(input.RenterId))
            {
                outputPort.RenterAlreadyHasVehicleHandle($"The person {input.RenterId} already has a vehicle rented.");
                return;
            }

            vehicle.Rent(input.RenterId, timeProvider.GetUtcNow().UtcDateTime);

            try
            {
                await vehicleRepository.Update(vehicle);
            }
            catch (RenterAlreadyHasVehicleException ex)
            {
                // A concurrent request has rented another vehicle to the same person (detected by the persistence adapter).
                outputPort.RenterAlreadyHasVehicleHandle(ex.Message);
                return;
            }

            outputPort.StandardHandle(new RentVehicleOutput(VehicleOutput.From(vehicle)));
        }
    }
}
