using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.UseCases;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.CreateVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.GetAvailableVehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.ReturnVehicle;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Specs
{
    /// <summary>
    /// Functional (integration) tests: the real composition root is used (MediatR handlers, use cases,
    /// presenters and the in-memory repository) but without the web host. Requests are sent straight
    /// to the MediatR handlers.
    /// </summary>
    public sealed class VehicleRentalFunctionalTests(CompositionRootTestFixture fixture) : FunctionalTestBase(fixture)
    {
        [Fact]
        public async Task Vehicle_CanBeCreatedRentedAndReturned()
        {
            var vehicle = await CreateVehicle(DateTime.UtcNow.AddYears(-1));
            var renterId = NewRenterId();

            var rentResult = await Send(new RentVehicleRequest(vehicle.Id, renterId));

            var rented = rentResult.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<VehicleResponse>().Subject;
            rented.Status.Should().Be("Rented");
            rented.RentedBy.Should().Be(renterId);
            (await GetAvailableVehicles()).Should().NotContain(v => v.Id == vehicle.Id);

            var returnResult = await Send(new ReturnVehicleRequest(vehicle.Id));

            var returned = returnResult.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<ReturnVehicleResponse>().Subject;
            returned.RenterId.Should().Be(renterId);
            returned.Vehicle.Status.Should().Be("Available");
            (await GetAvailableVehicles()).Should().Contain(v => v.Id == vehicle.Id);
        }

        [Fact]
        public async Task SamePerson_CanNotRentTwoVehiclesAtTheSameTime()
        {
            var first = await CreateVehicle(DateTime.UtcNow.AddYears(-1));
            var second = await CreateVehicle(DateTime.UtcNow.AddYears(-2));
            var renterId = NewRenterId();

            await Send(new RentVehicleRequest(first.Id, renterId));
            var result = await Send(new RentVehicleRequest(second.Id, renterId));

            result.Should().BeOfType<ConflictObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            (await GetAvailableVehicles()).Should().Contain(v => v.Id == second.Id);
        }

        [Fact]
        public async Task VehicleOlderThanFiveYears_CanNotBeAddedToTheFleet()
        {
            var act = () => CreateVehicle(DateTime.UtcNow.AddYears(-6));

            await act.Should().ThrowAsync<VehicleTooOldException>();
        }

        private static string NewRenterId() => $"R-{Guid.NewGuid():N}"[..12].ToUpperInvariant();

        private async Task<VehicleResponse> CreateVehicle(DateTime manufactureDate)
        {
            var result = await Send(new CreateVehicleRequest
            {
                Brand = "Toyota",
                Model = "Corolla",
                LicensePlate = Guid.NewGuid().ToString("N")[..10],
                ManufactureDate = manufactureDate,
            });

            var created = result.Should().BeOfType<ObjectResult>().Subject;
            created.StatusCode.Should().Be(StatusCodes.Status201Created);
            return created.Value.Should().BeOfType<VehicleResponse>().Subject;
        }

        private async Task<VehicleResponse[]> GetAvailableVehicles()
        {
            var result = await Send(new GetAvailableVehiclesRequest());
            return result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeAssignableTo<IEnumerable<VehicleResponse>>().Subject.ToArray();
        }

        private async Task<IActionResult> Send<TRequest>(TRequest request)
            where TRequest : IRequest<IWebApiPresenter>
        {
            IActionResult actionResult = null;

            await Fixture.UsingHandlerForRequestResponse<TRequest, IWebApiPresenter>(async handler =>
            {
                var presenter = await handler.Handle(request, CancellationToken.None);
                actionResult = presenter.ActionResult;
            });

            return actionResult;
        }
    }
}
