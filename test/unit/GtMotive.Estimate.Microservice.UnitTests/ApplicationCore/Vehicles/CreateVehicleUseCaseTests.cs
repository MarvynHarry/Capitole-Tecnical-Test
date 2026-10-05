using System;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.UnitTests.TestDoubles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Vehicles
{
    public class CreateVehicleUseCaseTests
    {
        private readonly Mock<IVehicleRepository> _repository = new();
        private readonly Mock<ICreateVehicleOutputPort> _outputPort = new();
        private readonly CreateVehicleUseCase _sut;

        public CreateVehicleUseCaseTests()
        {
            _sut = new CreateVehicleUseCase(_repository.Object, _outputPort.Object, new FixedTimeProvider(VehicleMother.Today));
        }

        [Fact]
        public async Task Execute_WithValidVehicle_AddsItAndCallsStandardOutput()
        {
            var input = new CreateVehicleInput("Seat", "Ibiza", new LicensePlate("1234ABC"), VehicleMother.Today.AddYears(-1));

            await _sut.Execute(input);

            _repository.Verify(r => r.Add(It.Is<Vehicle>(v => v.LicensePlate == input.LicensePlate && v.IsAvailable)), Times.Once);
            _outputPort.Verify(p => p.StandardHandle(It.Is<CreateVehicleOutput>(o => o.Vehicle.LicensePlate == "1234ABC")), Times.Once);
        }

        [Fact]
        public async Task Execute_WhenLicensePlateAlreadyExists_CallsLicensePlateAlreadyExistsOutput()
        {
            _repository.Setup(r => r.ExistsWithLicensePlate(new LicensePlate("1234ABC"))).ReturnsAsync(true);
            var input = new CreateVehicleInput("Seat", "Ibiza", new LicensePlate("1234ABC"), VehicleMother.Today.AddYears(-1));

            await _sut.Execute(input);

            _outputPort.Verify(p => p.LicensePlateAlreadyExistsHandle(It.IsAny<string>()), Times.Once);
            _repository.Verify(r => r.Add(It.IsAny<Vehicle>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenVehicleIsOlderThanFiveYears_ThrowsAndDoesNotPersist()
        {
            var input = new CreateVehicleInput("Seat", "Ibiza", new LicensePlate("1234ABC"), VehicleMother.Today.AddYears(-6));

            var act = () => _sut.Execute(input);

            await act.Should().ThrowAsync<VehicleTooOldException>();
            _repository.Verify(r => r.Add(It.IsAny<Vehicle>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenRepositoryDetectsDuplicatedPlate_CallsLicensePlateAlreadyExistsOutput()
        {
            _repository.Setup(r => r.Add(It.IsAny<Vehicle>())).ThrowsAsync(new LicensePlateAlreadyExistsException("duplicated"));
            var input = new CreateVehicleInput("Seat", "Ibiza", new LicensePlate("1234ABC"), VehicleMother.Today.AddYears(-1));

            await _sut.Execute(input);

            _outputPort.Verify(p => p.LicensePlateAlreadyExistsHandle("duplicated"), Times.Once);
            _outputPort.Verify(p => p.StandardHandle(It.IsAny<CreateVehicleOutput>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WithNullInput_ThrowsArgumentNullException()
        {
            var act = () => _sut.Execute(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
