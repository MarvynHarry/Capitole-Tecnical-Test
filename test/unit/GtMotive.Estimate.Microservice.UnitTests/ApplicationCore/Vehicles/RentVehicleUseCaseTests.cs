using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.RentVehicle;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.UnitTests.TestDoubles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Vehicles
{
    /// <summary>
    /// Unit tests of the Rent Vehicle use case. The repository and the output port are mocked,
    /// so the use case is validated in isolation (no host, no database, no MediatR).
    /// </summary>
    public class RentVehicleUseCaseTests
    {
        private static readonly RenterId Renter = new("12345678Z");

        private readonly Mock<IVehicleRepository> _repository = new();
        private readonly Mock<IRentVehicleOutputPort> _outputPort = new();
        private readonly RentVehicleUseCase _sut;

        public RentVehicleUseCaseTests()
        {
            _sut = new RentVehicleUseCase(_repository.Object, _outputPort.Object, new FixedTimeProvider(VehicleMother.Today));
        }

        [Fact]
        public async Task Execute_WhenVehicleIsAvailableAndRenterHasNoRental_RentsAndPersistsVehicle()
        {
            var vehicle = VehicleMother.Available();
            _repository.Setup(r => r.GetById(vehicle.Id)).ReturnsAsync(vehicle);

            await _sut.Execute(new RentVehicleInput(vehicle.Id, Renter));

            _repository.Verify(r => r.Update(It.Is<Vehicle>(v => v.Status == VehicleStatus.Rented && v.CurrentRental.Renter == Renter)), Times.Once);
            _outputPort.Verify(p => p.StandardHandle(It.Is<RentVehicleOutput>(o => o.Vehicle.RentedBy == "12345678Z")), Times.Once);
        }

        [Fact]
        public async Task Execute_WhenRenterAlreadyHasAVehicle_CallsRenterAlreadyHasVehicleOutput()
        {
            var vehicle = VehicleMother.Available();
            _repository.Setup(r => r.GetById(vehicle.Id)).ReturnsAsync(vehicle);
            _repository.Setup(r => r.HasActiveRental(Renter)).ReturnsAsync(true);

            await _sut.Execute(new RentVehicleInput(vehicle.Id, Renter));

            _outputPort.Verify(p => p.RenterAlreadyHasVehicleHandle(It.IsAny<string>()), Times.Once);
            _repository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenVehicleDoesNotExist_CallsNotFoundOutput()
        {
            await _sut.Execute(new RentVehicleInput(VehicleId.New(), Renter));

            _outputPort.Verify(p => p.NotFoundHandle(It.IsAny<string>()), Times.Once);
            _repository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenVehicleIsAlreadyRented_CallsVehicleNotAvailableOutput()
        {
            var vehicle = VehicleMother.RentedBy("99999999R");
            _repository.Setup(r => r.GetById(vehicle.Id)).ReturnsAsync(vehicle);

            await _sut.Execute(new RentVehicleInput(vehicle.Id, Renter));

            _outputPort.Verify(p => p.VehicleNotAvailableHandle(It.IsAny<string>()), Times.Once);
            _repository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenPersistenceDetectsConcurrentRental_CallsRenterAlreadyHasVehicleOutput()
        {
            var vehicle = VehicleMother.Available();
            _repository.Setup(r => r.GetById(vehicle.Id)).ReturnsAsync(vehicle);
            _repository.Setup(r => r.Update(vehicle)).ThrowsAsync(new RenterAlreadyHasVehicleException("concurrent"));

            await _sut.Execute(new RentVehicleInput(vehicle.Id, Renter));

            _outputPort.Verify(p => p.RenterAlreadyHasVehicleHandle("concurrent"), Times.Once);
            _outputPort.Verify(p => p.StandardHandle(It.IsAny<RentVehicleOutput>()), Times.Never);
        }
    }
}
