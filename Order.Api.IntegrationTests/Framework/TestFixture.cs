using System;
using System.IO;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Order.Application;
using Order.Application.Mapper;
using Order.Domain;
using Order.EventSubscribers.Subscribers;
using Order.Infrastructure;

namespace Order.Api.IntegrationTests.Framework
{
    public sealed class TestFixture
    {
        private static readonly Lazy<TestFixture> lazy = new Lazy<TestFixture>(() => new TestFixture());
        private readonly ServiceProvider _serviceProvider;
        private TestFixture()
        {
            var serviceCollection = new ServiceCollection();
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Testing.json").Build();
            serviceCollection.AddInfrastructureServices(config);
            serviceCollection.AddDomainServices();
            serviceCollection.AddApplicationServices();
            
            serviceCollection.AddHostedService<ReadModelWriter>();
            serviceCollection.AddSingleton<ReadModelWriter>();
            
            serviceCollection.AddHostedService<PaymentNotifyWorker>();
            serviceCollection.AddSingleton<PaymentNotifyWorker>();
            
            serviceCollection.AddHostedService<AggregaterootWriter>();
            serviceCollection.AddSingleton<AggregaterootWriter>();
            
            serviceCollection.AddHostedService<TransactionCountReporterWorker>();
            serviceCollection.AddSingleton<TransactionCountReporterWorker>();
            
            serviceCollection.AddSingleton<IMapper>(s => (new MapperConfiguration
                (c => { c.AddProfile<CommandMapperProfile>(); })).CreateMapper());
            serviceCollection.AddLogging(logging => logging.AddConsole());
            _serviceProvider = serviceCollection.BuildServiceProvider();

        }
        public static TestFixture Instance => lazy.Value;

        public T GetRequiredService<T>() => _serviceProvider.GetRequiredService<T>();
    }
}

