using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.RentVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.RentVehicle;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.UnitTests.TestDoubles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.Api.Vehicles
{
    public class RentVehiclePresenterTests
    {
        private readonly RentVehiclePresenter _sut = new();

        [Fact]
        public void StandardHandle_ReturnsOkWithVehicle()
        {
            var vehicle = VehicleMother.RentedBy("12345678Z");

            _sut.StandardHandle(new RentVehicleOutput(VehicleOutput.From(vehicle)));

            var result = _sut.ActionResult.Should().BeOfType<OkObjectResult>().Subject;
            var body = result.Value.Should().BeOfType<VehicleResponse>().Subject;
            body.Status.Should().Be(nameof(VehicleStatus.Rented));
            body.RentedBy.Should().Be("12345678Z");
        }

        [Fact]
        public void NotFoundHandle_ReturnsNotFound()
        {
            _sut.NotFoundHandle("not found");

            _sut.ActionResult.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeOfType<ProblemDetails>()
                .Which.Detail.Should().Be("not found");
        }

        [Fact]
        public void RenterAlreadyHasVehicleHandle_ReturnsConflict()
        {
            _sut.RenterAlreadyHasVehicleHandle("conflict");

            _sut.ActionResult.Should().BeOfType<ConflictObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        }

        [Fact]
        public void VehicleNotAvailableHandle_ReturnsConflict()
        {
            _sut.VehicleNotAvailableHandle("conflict");

            _sut.ActionResult.Should().BeOfType<ConflictObjectResult>();
        }
    }
}
