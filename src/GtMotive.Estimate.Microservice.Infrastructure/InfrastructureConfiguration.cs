using System;
using System.Diagnostics.CodeAnalysis;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Infrastructure.Interfaces;
using GtMotive.Estimate.Microservice.Infrastructure.Logging;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using GtMotive.Estimate.Microservice.Infrastructure.Repositories.InMemory;
using GtMotive.Estimate.Microservice.Infrastructure.Repositories.MongoDb;
using GtMotive.Estimate.Microservice.Infrastructure.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.Infrastructure
{
    public static class InfrastructureConfiguration
    {
        [ExcludeFromCodeCoverage]
        public static IInfrastructureBuilder AddBaseInfrastructure(
            this IServiceCollection services,
            bool isDevelopment)
        {
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

            if (!isDevelopment)
            {
                services.AddScoped<ITelemetry, AppTelemetry>();
            }
            else
            {
                services.AddScoped<ITelemetry, NoOpTelemetry>();
            }

            services.TryAddSingleton(TimeProvider.System);

            // Default persistence: in memory. Replaced by MongoDB when AddMongoDbPersistence is called.
            services.TryAddSingleton<IVehicleRepository, InMemoryVehicleRepository>();

            return new InfrastructureBuilder(services);
        }

        /// <summary>
        /// Uses MongoDB as persistence instead of the in-memory default.
        /// Requires <see cref="MongoDb.Settings.MongoDbSettings"/> to be configured.
        /// </summary>
        /// <param name="builder">Infrastructure builder.</param>
        /// <returns>The same builder.</returns>
        [ExcludeFromCodeCoverage]
        public static IInfrastructureBuilder AddMongoDbPersistence(this IInfrastructureBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.Services.TryAddSingleton<MongoService>();
            builder.Services.Replace(ServiceDescriptor.Singleton<IVehicleRepository, MongoVehicleRepository>());

            return builder;
        }

        private sealed class InfrastructureBuilder(IServiceCollection services) : IInfrastructureBuilder
        {
            public IServiceCollection Services { get; } = services;
        }
    }
}
