using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Order.Domain.AggregateRoot;
using Order.Domain.ReadModels;
using Order.Domain.Repositories;
using Order.Infrastructure;
using Order.Infrastructure.AppSettingsConfigration;
using Order.Infrastructure.EventStore;
using Order.Infrastructure.ReadModel;

namespace Order.EventSubscribers.Subscribers
{
    public class AggregaterootWriter : BackgroundService
    {
        private readonly ILogger<AggregaterootWriter> _logger;
        private readonly EventStoreAdapter _eventStoreAdapter;
        private readonly IEventRepository<OrderAggregateRoot> _eventRepository;
        private readonly string _groupName = $"AggregaterootWriter-{DateTime.Now.ToLongTimeString()}";
        private string _streamName = $"$ce-{Constants.ORDER_STREAM_NAME}";
        private readonly IDomainReadModelRepository<OrderReadModel> _domainReadModelRepository;


        public AggregaterootWriter(ILogger<AggregaterootWriter> logger, EventStoreAdapter eventStoreAdapter,
            IDomainReadModelRepository<OrderReadModel> domainReadModelRepository,IEventRepository<OrderAggregateRoot> eventRepository)
        {
            _logger = logger;
            _eventStoreAdapter = eventStoreAdapter;
            _domainReadModelRepository = domainReadModelRepository;
            _eventRepository = eventRepository;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            // try
            // {
            //     await _eventStoreAdapter.DeletePersistentSubscriptionAsync(StreamName, GroupName,
            //         new UserCredentials(EventStoreConfigration.EventStoreConfig.UserName,
            //             EventStoreConfigration.EventStoreConfig.Password));
            // }
            // catch (Exception e)
            // {
            //     _logger.LogInformation(e.Message.ToString());
            // }
            try
            {
                var persistentSubscriptionSettingsBuilder = PersistentSubscriptionSettings.Create();
                persistentSubscriptionSettingsBuilder.ResolveLinkTos();
                persistentSubscriptionSettingsBuilder.StartFromBeginning();
                await _eventStoreAdapter.CreatePersistentSubscriptionAsync(
                    _streamName, _groupName,
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
            await _eventStoreAdapter.ConnectToPersistentSubscriptionAsync(
                _streamName, _groupName,
                (sub, e) => { ReadEvent(sub, e); },
                (sub, reason, ex) => { ReadExEvent(sub, reason, ex); },
                new UserCredentials(EventStoreConfigration.EventStoreConfig.UserName,
                    EventStoreConfigration.EventStoreConfig.Password)
                , 10, false
            );
        }

        private void ReadExEvent(EventStorePersistentSubscriptionBase sub, SubscriptionDropReason reason,
            Exception arg3)
        {
            _logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(reason.ToString()));
        }

        private async Task ReadEvent(EventStorePersistentSubscriptionBase sub, ResolvedEvent resolvedEvent)
        {
            try
            {
                var eventType = resolvedEvent.Event.EventType;
                var streamId = resolvedEvent.Event.EventStreamId.Replace($"{Constants.ORDER_STREAM_NAME}-","");

                var aggregateRoot = await _eventRepository.GetAggregateRootSnapshot(Guid.Parse(streamId));

                await _eventRepository.AddSnapshotToDb(aggregateRoot);
                _logger.LogInformation($"AggregaterootWriter - Aggregateroot upserted id : {aggregateRoot.Id.ToString()}");
                sub.Acknowledge(resolvedEvent.OriginalEvent.EventId);
            }
            catch (Exception e)
            {
                _logger.LogInformation(
                    $"event can not read {resolvedEvent.Event.EventStreamId} | message {e.Message.ToString()}");
                sub.Fail(resolvedEvent, PersistentSubscriptionNakEventAction.Park,
                    $"event consume fail. {DateTime.Now.ToString()} | message {e.Message.ToString()} ");
            }
        }
    }
}