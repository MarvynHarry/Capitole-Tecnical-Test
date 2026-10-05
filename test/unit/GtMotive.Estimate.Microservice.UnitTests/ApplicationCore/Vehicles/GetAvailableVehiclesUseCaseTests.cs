using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetAvailableVehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.UnitTests.TestDoubles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Vehicles
{
    public class GetAvailableVehiclesUseCaseTests
    {
        [Fact]
        public async Task Execute_ExcludesVehiclesOlderThanFiveYears()
        {
            var recent = VehicleMother.Available("1111AAA", VehicleMother.Today.AddYears(-1));
            var tooOld = VehicleMother.Available("2222BBB", VehicleMother.Today.AddYears(-6));

            var repository = new Mock<IVehicleRepository>();
            repository.Setup(r => r.GetAvailable()).ReturnsAsync([recent, tooOld]);

            GetAvailableVehiclesOutput output = null;
            var outputPort = new Mock<IGetAvailableVehiclesOutputPort>();
            outputPort.Setup(p => p.StandardHandle(It.IsAny<GetAvailableVehiclesOutput>())).Callback<GetAvailableVehiclesOutput>(o => output = o);

            var sut = new GetAvailableVehiclesUseCase(repository.Object, outputPort.Object, new FixedTimeProvider(VehicleMother.Today));

            await sut.Execute(new GetAvailableVehiclesInput());

            output.Vehicles.Select(v => v.LicensePlate).Should().ContainSingle().Which.Should().Be("1111AAA");
        }
    }
}
