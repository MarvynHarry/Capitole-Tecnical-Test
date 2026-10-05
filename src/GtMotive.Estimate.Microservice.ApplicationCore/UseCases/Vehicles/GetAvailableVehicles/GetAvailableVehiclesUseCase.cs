using System;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetAvailableVehicles
{
    /// <summary>
    /// Lists the vehicles of the fleet that can be rented right now. Vehicles that have
    /// exceeded the maximum age of the fleet are not listed.
    /// </summary>
    /// <param name="vehicleRepository">Vehicle repository.</param>
    /// <param name="outputPort">Output port.</param>
    /// <param name="timeProvider">Clock.</param>
    public sealed class GetAvailableVehiclesUseCase(
        IVehicleRepository vehicleRepository,
        IGetAvailableVehiclesOutputPort outputPort,
        TimeProvider timeProvider) : IUseCase<GetAvailableVehiclesInput>
    {
        /// <inheritdoc />
        public async Task Execute(GetAvailableVehiclesInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var today = timeProvider.GetUtcNow().UtcDateTime;
            var vehicles = await vehicleRepository.GetAvailable();

            var available = vehicles
                .Where(vehicle => vehicle.IsEligibleForFleet(today))
                .OrderBy(vehicle => vehicle.Brand, StringComparer.OrdinalIgnoreCase)
                .ThenBy(vehicle => vehicle.Model, StringComparer.OrdinalIgnoreCase)
                .Select(VehicleOutput.From)
                .ToList();

            outputPort.StandardHandle(new GetAvailableVehiclesOutput(available));
        }
    }
}
