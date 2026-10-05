using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle
{
    /// <summary>
    /// Adds a new vehicle to the fleet.
    /// </summary>
    /// <param name="vehicleRepository">Vehicle repository.</param>
    /// <param name="outputPort">Output port.</param>
    /// <param name="timeProvider">Clock.</param>
    public sealed class CreateVehicleUseCase(
        IVehicleRepository vehicleRepository,
        ICreateVehicleOutputPort outputPort,
        TimeProvider timeProvider) : IUseCase<CreateVehicleInput>
    {
        /// <inheritdoc />
        public async Task Execute(CreateVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (await vehicleRepository.ExistsWithLicensePlate(input.LicensePlate))
            {
                outputPort.LicensePlateAlreadyExistsHandle($"A vehicle with license plate {input.LicensePlate} already exists in the fleet.");
                return;
            }

            var vehicle = Vehicle.Create(
                input.Brand,
                input.Model,
                input.LicensePlate,
                input.ManufactureDate,
                timeProvider.GetUtcNow().UtcDateTime);

            try
            {
                await vehicleRepository.Add(vehicle);
            }
            catch (LicensePlateAlreadyExistsException ex)
            {
                // A concurrent request has added the same license plate (detected by the persistence adapter).
                outputPort.LicensePlateAlreadyExistsHandle(ex.Message);
                return;
            }

            outputPort.StandardHandle(new CreateVehicleOutput(VehicleOutput.From(vehicle)));
        }
    }
}
