using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.ReturnVehicle;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.UnitTests.TestDoubles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Vehicles
{
    public class ReturnVehicleUseCaseTests
    {
        private readonly Mock<IVehicleRepository> _repository = new();
        private readonly Mock<IReturnVehicleOutputPort> _outputPort = new();
        private readonly ReturnVehicleUseCase _sut;

        public ReturnVehicleUseCaseTests()
        {
            _sut = new ReturnVehicleUseCase(_repository.Object, _outputPort.Object, new FixedTimeProvider(VehicleMother.Today));
        }

        [Fact]
        public async Task Execute_WhenVehicleIsRented_ReturnsAndPersistsVehicle()
        {
            var vehicle = VehicleMother.RentedBy("12345678Z");
            _repository.Setup(r => r.GetById(vehicle.Id)).ReturnsAsync(vehicle);

            await _sut.Execute(new ReturnVehicleInput(vehicle.Id));

            _repository.Verify(r => r.Update(It.Is<Vehicle>(v => v.IsAvailable)), Times.Once);
            _outputPort.Verify(
                p => p.StandardHandle(It.Is<ReturnVehicleOutput>(o => o.RenterId == "12345678Z" && o.ReturnedAt == VehicleMother.Today)),
                Times.Once);
        }

        [Fact]
        public async Task Execute_WhenVehicleIsNotRented_CallsVehicleNotRentedOutput()
        {
            var vehicle = VehicleMother.Available();
            _repository.Setup(r => r.GetById(vehicle.Id)).ReturnsAsync(vehicle);

            await _sut.Execute(new ReturnVehicleInput(vehicle.Id));

            _outputPort.Verify(p => p.VehicleNotRentedHandle(It.IsAny<string>()), Times.Once);
            _repository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenVehicleDoesNotExist_CallsNotFoundOutput()
        {
            await _sut.Execute(new ReturnVehicleInput(VehicleId.New()));

            _outputPort.Verify(p => p.NotFoundHandle(It.IsAny<string>()), Times.Once);
        }
    }
}
