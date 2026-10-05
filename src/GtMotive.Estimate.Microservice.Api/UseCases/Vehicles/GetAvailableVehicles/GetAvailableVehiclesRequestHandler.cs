using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetAvailableVehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.GetAvailableVehicles
{
    public sealed class GetAvailableVehiclesRequestHandler(
        IUseCase<GetAvailableVehiclesInput> useCase,
        GetAvailableVehiclesPresenter presenter) : IRequestHandler<GetAvailableVehiclesRequest, IWebApiPresenter>
    {
        public async Task<IWebApiPresenter> Handle(GetAvailableVehiclesRequest request, CancellationToken cancellationToken)
        {
            await useCase.Execute(new GetAvailableVehiclesInput());

            return presenter;
        }
    }
}
