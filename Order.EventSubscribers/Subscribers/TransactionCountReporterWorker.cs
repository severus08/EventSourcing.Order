using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Order.Domain.Events;
using Order.Domain.ReadModels;
using Order.Domain.Repositories;
using Order.Infrastructure;
using Order.Infrastructure.AppSettingsConfigration;
using Order.Infrastructure.EventStore;
using Order.Infrastructure.Mongo;
using Order.Infrastructure.ReadModel;

namespace Order.EventSubscribers.Subscribers
{
    public class TransactionCountReporterWorker : BackgroundService
    {
        private readonly ILogger<TransactionCountReporterWorker> _logger;
        private readonly EventStoreAdapter _eventStoreAdapter;
        private readonly string _groupName = $"TransactionCountReporterSubscriber-{DateTime.Now.ToLongTimeString()}";

        private readonly string _defaultReportStreamsProjectionName =
            ReportProjectionQueries.TransactionCountReporterStreamName;

        private readonly EventStoreProjectionAdapter _eventStoreProjectionAdapter;
        private readonly IDomainReadModelRepository<OrderReadModel> _domainReadModelRepository;
        private readonly IMongoRepository _mongoRepository;

        public TransactionCountReporterWorker(ILogger<TransactionCountReporterWorker> logger
            , EventStoreAdapter eventStoreAdapter
            , EventStoreProjectionAdapter eventStoreProjectionAdapter
            , IDomainReadModelRepository<OrderReadModel> domainReadModelRepository
            , IMongoRepository mongoRepository)
        {
            _logger = logger;
            _eventStoreAdapter = eventStoreAdapter;
            _eventStoreProjectionAdapter = eventStoreProjectionAdapter;
            _domainReadModelRepository = domainReadModelRepository;
            _mongoRepository = mongoRepository;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            // try
            // {
            //     await _eventStoreAdapter.DeletePersistentSubscriptionAsync(DefaultReportStreamsProjectionName,
            //         GroupName,
            //         new UserCredentials(EventStoreConfigration.EventStoreConfig.UserName,
            //             EventStoreConfigration.EventStoreConfig.Password));
            // }
            // catch (Exception e)
            // {
            //     _logger.LogInformation(e.Message.ToString());
            // }

            try
            {
                await _eventStoreProjectionAdapter.UpsertCustomProjections(Projections
                    .RegisteredReportedProjections);

                var persistentSubscriptionSettingsBuilder = PersistentSubscriptionSettings.Create();
                persistentSubscriptionSettingsBuilder.ResolveLinkTos();
                persistentSubscriptionSettingsBuilder.StartFromBeginning();

                await _eventStoreAdapter.CreatePersistentSubscriptionAsync(
                    _defaultReportStreamsProjectionName, _groupName,
                    persistentSubscriptionSettingsBuilder,
                    new UserCredentials(EventStoreConfigration.EventStoreConfig.UserName,
                        EventStoreConfigration.EventStoreConfig.Password)
                );
            }
            catch (System.InvalidOperationException e)
            {
                _logger.LogInformation("Subscriber already exists. :)");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _eventStoreAdapter.ConnectToPersistentSubscriptionAsync(_defaultReportStreamsProjectionName,
                _groupName,
                (sub, e) => { ReadEvent(sub, e); },
                (sub, reason, ex) => { ReadExEvent(sub, reason, ex); },
                new UserCredentials(EventStoreConfigration.EventStoreConfig.UserName,
                    EventStoreConfigration.EventStoreConfig.Password)
                , 10, false
            );
            Console.WriteLine($"connected {DateTime.Now.ToLongTimeString()}");
        }

        private void ReadExEvent(EventStorePersistentSubscriptionBase sub, SubscriptionDropReason reason,
            Exception arg3)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(reason.ToString()));
        }

        private async Task ReadEvent(EventStorePersistentSubscriptionBase sub, ResolvedEvent resolvedEvent)
        {
            try
            {
                var eventType = resolvedEvent.Event.EventType;
                var serilazedEvent = Encoding.UTF8.GetString(@resolvedEvent.Event.Data);
                var eventStreamId = resolvedEvent.Event.EventStreamId;

                var readModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TransactionCountReportReadModel>(
                    Encoding.UTF8.GetString(@resolvedEvent.Event.Data));
                readModel.CreatedOn = DateTime.Now;

                await _mongoRepository.InsertOneAsync<TransactionCountReportReadModel>(readModel);

                sub.Acknowledge(resolvedEvent.OriginalEvent.EventId);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    $"event can not read {resolvedEvent.Event.EventStreamId} | message {e.Message.ToString()}");
                sub.Fail(resolvedEvent, PersistentSubscriptionNakEventAction.Park,
                    $"event consume fail. {DateTime.Now.ToString()} | message {e.Message.ToString()} ");
            }
        }
    }
}