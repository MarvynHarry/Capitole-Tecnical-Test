using System;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.RentVehicle
{
    /// <summary>
    /// Request to rent a vehicle.
    /// </summary>
    /// <param name="VehicleId">Vehicle to rent.</param>
    /// <param name="RenterId">Person that rents the vehicle.</param>
    public sealed record RentVehicleRequest(Guid VehicleId, string RenterId) : IRequest<IWebApiPresenter>;
}
