using System.Collections.Generic;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.CommandHandlers;
using Order.Application.Commands;
using Order.Application.Queries;
using Order.Application.QueryHandlers;
using Order.Domain.ReadModels;
using Order.Infrastructure.EventStore;

namespace Order.Application
{
    public static class Config
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddCommandHandlers();
            services.AddQueryHandlers();
        }

        private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            return services.AddScoped<IRequestHandler<CreateOrderCommand, Unit>, OrderCommandHandler>()
                .AddScoped<IRequestHandler<CancelOrderCommand, Unit>, OrderCommandHandler>()
                .AddScoped<IRequestHandler<PaymentApproveCommand, Unit>, OrderPaymentCommandHandler>()
                .AddScoped<IRequestHandler<PaymentRejectCommand, Unit>, OrderPaymentCommandHandler>()
                .AddScoped<IRequestHandler<ShipmentStartCommand, Unit>, OrderShipmentCommandHandler>()
                .AddScoped<IRequestHandler<ShipmentFinishCommand, Unit>, OrderShipmentCommandHandler>();
        }

        private static IServiceCollection AddQueryHandlers(this IServiceCollection services)
        {
            return services.AddScoped<IRequestHandler<GetOrderQuery, OrderReadModel>, OrderQueryHandler>()
                .AddScoped<IRequestHandler<GetOrderListQuery, List<OrderReadModel>>, OrderQueryHandler>();
        }
    }
}