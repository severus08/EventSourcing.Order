using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Order.Domain.AggregateRoot;
using Order.Domain.Events;
using Order.Domain.Factory;
using Order.Domain.ReadModels;
using Order.Domain.Repositories;
using Order.Infrastructure.Domain;
using Order.Infrastructure.EventStore;
using Order.Infrastructure.Factories;
using Order.Infrastructure.ReadModel;

namespace Order.Domain
{
    public static class Config
    {
        public static void AddDomainServices(this IServiceCollection services) 
        { 
            services.AddScoped<IDomainFactory<OrderAggregateRoot>, OrderFactory>();
            services.AddScoped<IDomainService<OrderAggregateRoot>, OrderDomainService>();
            services.AddSingleton<IEventRepository<OrderAggregateRoot>, OrderAggregateRootRepository>();
            services.AddSingleton<IDomainReadModelRepository<OrderReadModel>, OrderReadModelRepository>();
        }
        public static void RegisterApplicationLifetimeMethods(this IHostApplicationLifetime lifetime,IApplicationBuilder app)
        {
            lifetime.ApplicationStarted.Register(() =>
            {
                Console.WriteLine("app starting!!");
                app.ApplicationServices.GetService<EventStoreProjectionAdapter>().UpsertCustomProjections(Projections.RegisteredProjections);
            });
        }
    }
}