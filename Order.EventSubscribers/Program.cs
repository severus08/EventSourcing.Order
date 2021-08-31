using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Order.Application;
using Order.Application.Mapper;
using Order.Domain;
using Order.EventSubscribers.Subscribers;
using Order.Infrastructure;

namespace Order.EventSubscribers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddInfrastructureServices(hostContext.Configuration);
                    services.AddDomainServices();
                    services.AddApplicationServices();
                    services.AddHostedService<ReadModelWriter>();
                    services.AddHostedService<PaymentNotifyWorker>();
                    services.AddHostedService<AggregaterootWriter>();
                    services.AddHostedService<TransactionCountReporterWorker>();
                    services.AddSingleton<IMapper>(s => (new MapperConfiguration
                        (c => { c.AddProfile<CommandMapperProfile>(); })).CreateMapper());
                });
    }
}