using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.ReturnVehicle
{
    /// <summary>
    /// Returns a rented vehicle to the fleet.
    /// </summary>
    /// <param name="vehicleRepository">Vehicle repository.</param>
    /// <param name="outputPort">Output port.</param>
    /// <param name="timeProvider">Clock.</param>
    public sealed class ReturnVehicleUseCase(
        IVehicleRepository vehicleRepository,
        IReturnVehicleOutputPort outputPort,
        TimeProvider timeProvider) : IUseCase<ReturnVehicleInput>
    {
        /// <inheritdoc />
        public async Task Execute(ReturnVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var vehicle = await vehicleRepository.GetById(input.VehicleId);

            if (vehicle is null)
            {
                outputPort.NotFoundHandle($"The vehicle {input.VehicleId} does not exist.");
                return;
            }

            if (vehicle.IsAvailable)
            {
                outputPort.VehicleNotRentedHandle($"The vehicle {input.VehicleId} is not rented.");
                return;
            }

            var finishedRental = vehicle.Return();

            await vehicleRepository.Update(vehicle);

            outputPort.StandardHandle(new ReturnVehicleOutput(
                VehicleOutput.From(vehicle),
                finishedRental.Renter.Value,
                finishedRental.StartedAt,
                timeProvider.GetUtcNow().UtcDateTime));
        }
    }
}
