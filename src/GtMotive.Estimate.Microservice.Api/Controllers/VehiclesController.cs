using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.CreateVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.GetAvailableVehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.ReturnVehicle;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers
{
    /// <summary>
    /// Fleet management: create, list, rent and return vehicles.
    /// </summary>
    /// <param name="mediator">Mediator used to dispatch the requests to their handlers.</param>
    [ApiController]
    [Route("api/vehicles")]
    [Produces("application/json")]
    public sealed class VehiclesController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Adds a new vehicle to the fleet. The manufacture date can not be older than 5 years.
        /// </summary>
        /// <param name="request">Vehicle data.</param>
        /// <returns>The created vehicle.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody][Required] CreateVehicleRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var presenter = await mediator.Send(request);
            return presenter.ActionResult;
        }

        /// <summary>
        /// Lists the vehicles of the fleet that are available to be rented.
        /// </summary>
        /// <returns>Available vehicles.</returns>
        [HttpGet("available")]
        [ProducesResponseType(typeof(VehicleResponse[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailable()
        {
            var presenter = await mediator.Send(new GetAvailableVehiclesRequest());
            return presenter.ActionResult;
        }

        /// <summary>
        /// Rents a vehicle to a person. A person can only have one vehicle rented at the same time.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="body">Person that rents the vehicle.</param>
        /// <returns>The rented vehicle.</returns>
        [HttpPost("{vehicleId:guid}/rent")]
        [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Rent([FromRoute] Guid vehicleId, [FromBody][Required] RentVehicleBody body)
        {
            ArgumentNullException.ThrowIfNull(body);

            var presenter = await mediator.Send(new RentVehicleRequest(vehicleId, body.RenterId));
            return presenter.ActionResult;
        }

        /// <summary>
        /// Returns a rented vehicle to the fleet.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <returns>Summary of the finished rental.</returns>
        [HttpPost("{vehicleId:guid}/return")]
        [ProducesResponseType(typeof(ReturnVehicleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Return([FromRoute] Guid vehicleId)
        {
            var presenter = await mediator.Send(new ReturnVehicleRequest(vehicleId));
            return presenter.ActionResult;
        }
    }
}
