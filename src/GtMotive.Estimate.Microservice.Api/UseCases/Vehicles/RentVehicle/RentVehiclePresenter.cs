using System;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.RentVehicle;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.RentVehicle
{
    public sealed class RentVehiclePresenter : IWebApiPresenter, IRentVehicleOutputPort
    {
        public IActionResult ActionResult { get; private set; }

        public void StandardHandle(RentVehicleOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            ActionResult = new OkObjectResult(new VehicleResponse(response.Vehicle));
        }

        public void NotFoundHandle(string message)
        {
            ActionResult = ProblemResults.NotFound(message);
        }

        public void VehicleNotAvailableHandle(string message)
        {
            ActionResult = ProblemResults.Conflict(message);
        }

        public void RenterAlreadyHasVehicleHandle(string message)
        {
            ActionResult = ProblemResults.Conflict(message);
        }
    }
}
