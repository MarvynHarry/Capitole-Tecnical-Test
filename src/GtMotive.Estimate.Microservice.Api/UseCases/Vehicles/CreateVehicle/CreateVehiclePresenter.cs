using System;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.CreateVehicle
{
    public sealed class CreateVehiclePresenter : IWebApiPresenter, ICreateVehicleOutputPort
    {
        public IActionResult ActionResult { get; private set; }

        public void StandardHandle(CreateVehicleOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            ActionResult = new ObjectResult(new VehicleResponse(response.Vehicle))
            {
                StatusCode = StatusCodes.Status201Created,
            };
        }

        public void LicensePlateAlreadyExistsHandle(string message)
        {
            ActionResult = ProblemResults.Conflict(message);
        }
    }
}
