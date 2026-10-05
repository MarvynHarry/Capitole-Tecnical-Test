using System;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.ReturnVehicle
{
    /// <summary>
    /// Request to return a rented vehicle.
    /// </summary>
    /// <param name="VehicleId">Vehicle to return.</param>
    public sealed record ReturnVehicleRequest(Guid VehicleId) : IRequest<IWebApiPresenter>;
}
