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
using Order.Infrastructure.ReadModel;

namespace Order.EventSubscribers.Subscribers
{
    public class PaymentNotifyWorker : BackgroundService
    {
        private readonly ILogger<PaymentNotifyWorker> _logger;
        private readonly EventStoreAdapter _eventStoreAdapter;
        private readonly string GroupName = $"PaymentNotifySubscriber-{DateTime.Now.ToLongTimeString()}";
        private readonly string _defaultPaymentStreamsProjectionName = $"$ce-{NotifyProjectionQueries.PaymentNotifyStreamName}";
        private readonly EventStoreProjectionAdapter _eventStoreProjectionAdapter;
        private readonly IDomainReadModelRepository<OrderReadModel> _domainReadModelRepository;

        public PaymentNotifyWorker(ILogger<PaymentNotifyWorker> logger
            , EventStoreAdapter eventStoreAdapter
            , EventStoreProjectionAdapter eventStoreProjectionAdapter
            , IDomainReadModelRepository<OrderReadModel> domainReadModelRepository)
        {
            _logger = logger;
            _eventStoreAdapter = eventStoreAdapter;
            _eventStoreProjectionAdapter = eventStoreProjectionAdapter;
            _domainReadModelRepository = domainReadModelRepository;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            // try
            // {
            //     await _eventStoreAdapter.DeletePersistentSubscriptionAsync(DefaultPaymentStreamsProjectionName, GroupName,
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
                    .RegisteredPaymentNotifyProjections);

                var persistentSubscriptionSettingsBuilder = PersistentSubscriptionSettings.Create();
                persistentSubscriptionSettingsBuilder.ResolveLinkTos();
                persistentSubscriptionSettingsBuilder.StartFromBeginning();
                
                await _eventStoreAdapter.CreatePersistentSubscriptionAsync(
                    _defaultPaymentStreamsProjectionName, GroupName,
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
            await _eventStoreAdapter.ConnectToPersistentSubscriptionAsync(_defaultPaymentStreamsProjectionName,
                GroupName,
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
                if (eventType == "PaymentApproved")
                {
                    var eventBody = Newtonsoft.Json.JsonConvert.DeserializeObject<EventBody>(
                        Encoding.UTF8.GetString(@resolvedEvent.Event.Data));
                    var eventData = (PaymentApproved) ((JObject) eventBody.DomainEvent).ToObject(typeof(PaymentApproved));
                    
                    await SendPaymentApprovedSms(eventStreamId, eventData?.PaymentId.ToString(), eventData.Amount);
                }

                if (eventType == "PaymentRejected")
                {
                    var eventBody = Newtonsoft.Json.JsonConvert.DeserializeObject<EventBody>(
                        Encoding.UTF8.GetString(@resolvedEvent.Event.Data));
                    var eventData = (PaymentRejected) ((JObject) eventBody.DomainEvent).ToObject(typeof(PaymentRejected));
                    await SendPaymentRejectedSms(eventStreamId, eventData?.PaymentId.ToString());
                }

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

        private async Task SendPaymentApprovedSms(string eventStreamId, string paymentId, decimal amount)
        {
            var streamId = Guid.Parse(eventStreamId.Replace($"{Constants.ORDER_STREAM_NAME}-", ""));
            var order = await _domainReadModelRepository.Get(x => x.OrderId == streamId);
            _logger.LogInformation($"PaymentNotifyWorker - payment successful. paymentId : {paymentId} | amount : {amount.ToString()}");
        }

        private async Task SendPaymentRejectedSms(string eventStreamId, string paymentId)
        {
            var streamId = Guid.Parse(eventStreamId.Replace($"{Constants.ORDER_STREAM_NAME}-", ""));
            var order = await _domainReadModelRepository.Get(x => x.OrderId == streamId);
            _logger.LogInformation($"PaymentNotifyWorker - payment failure. paymentId : {paymentId}");
        }
    }
}