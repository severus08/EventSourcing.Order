using System;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Infrastructure.AppSettingsConfigration;
using Order.Infrastructure.Command;
using Order.Infrastructure.EventStore;
using Order.Infrastructure.Query;
using Order.Infrastructure.Caching;
using Order.Infrastructure.Mongo;
using MongoAppConfigration = Order.Infrastructure.AppSettingsConfigration.MongoAppConfigration;

namespace Order.Infrastructure
{
    public static class Config
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICommandBus, CommandBus>();
            services.AddScoped<IQueryBus, QueryBus>();
            configuration.GetSection("EventStore").Bind(EventStoreConfigration.EventStoreConfig);
            configuration.GetSection("Mongo").Bind(MongoAppConfigration.MongoConfig);
            services.Configure<ProjectSettings>(configuration.GetSection("ProjectSettings"));

            services.AddSingleton(new EventStoreAdapter(
                $"{EventStoreConfigration.EventStoreConfig.Uri}:{EventStoreConfigration.EventStoreConfig.Port.ToString()}"
                , EventStoreConfigration.EventStoreConfig.UserName, EventStoreConfigration.EventStoreConfig.Password));
            services.AddSingleton(new EventStoreProjectionAdapter(EventStoreConfigration.EventStoreConfig.Uri,
                EventStoreConfigration.EventStoreConfig.ProjectionSetupPort));
            
            services.Configure<CacheConfigration>(configuration.GetSection("Cache"));
            services.AddEasyCaching(option =>
            {
                 option.UseInMemory(configuration,
                     configuration.GetSection("Cache").GetSection("InstanceName").Value);
            });
            
            services.AddScoped<ICacheManager, CacheManager>();

            services.AddSingleton<IMongoRepository, MongoRepository>();

            services.AddMediatR(Assembly.GetExecutingAssembly());
        }
    }
}