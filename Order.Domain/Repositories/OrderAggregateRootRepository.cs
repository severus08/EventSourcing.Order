using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Microsoft.Extensions.Options;
using Order.Domain.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Order.Domain.AggregateRoot;
using Order.Infrastructure.AppSettingsConfigration;
using Order.Infrastructure.Attribute;
using Order.Infrastructure.EventStore;
using Order.Infrastructure.Mongo;
using EventData = EventStore.ClientAPI.EventData;

namespace Order.Domain.Repositories
{
    public class OrderAggregateRootRepository : IEventRepository<OrderAggregateRoot>
    {
        private readonly IEventStoreConnection _connection;
        private readonly EventStoreProjectionAdapter _projectionAdapter;
        private readonly ProjectSettings _projectSettings;
        private readonly IMongoRepository _mongoRepository;

        private string _streamName;
        private List<Type> _domainEventTypes;
        private const string ORDER_PROJECTION_NAME = "order-agg-pro";
        public const string ORDER_PROJECTION_READ_MODEL_STREAM_NAME = "order-agg-pro-readmodels";
        public const string ORDER_PROJECTION_Version = "1";
        public static readonly string ORDER_PROJECTION_ORIGINAL_NAME = $"{ORDER_PROJECTION_NAME}-{ORDER_PROJECTION_Version}";

        private string StreamName
        {
            get
            {
                if (string.IsNullOrEmpty(_streamName))
                {
                    _streamName =
                        ((StreamNameAttribute) StreamNameAttribute.GetCustomAttribute(typeof(OrderAggregateRoot),
                            typeof(StreamNameAttribute))).StreamName;
                }

                return _streamName;
            }
        }

        public List<Type> DomainEventTypes
        {
            get
            {
                if (_domainEventTypes == default(List<Type>))
                {
                    _domainEventTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                        .Where(x => x.BaseType == typeof(OrderBaseEvent)).ToList();
                }

                return _domainEventTypes;
            }
        }

        public OrderAggregateRootRepository(EventStoreAdapter connection,
            EventStoreProjectionAdapter projectionAdapter,
            IOptions<ProjectSettings> option,
            IMongoRepository mongoRepository)
        {
            _connection = connection;
            _projectionAdapter = projectionAdapter;
            ;
            _projectSettings = option.Value;
            _mongoRepository = mongoRepository;
        }


        public async Task<IList<CustomEventData>> GetAllEvents(Guid orderId)
        {
            var partitionName = StreamName + "-" + orderId;
            var eventList = new List<CustomEventData> { };

            StreamEventsSlice eventsSlice =
                await _connection.ReadStreamEventsForwardAsync(partitionName, 0, 4096, true);
            foreach (var eventsSliceEvent in eventsSlice.Events)
            {
                var metaData =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<EventMetaData>(
                        Encoding.UTF8.GetString(eventsSliceEvent.Event.Metadata));
                var body = Newtonsoft.Json.JsonConvert.DeserializeObject<EventBody>(
                    Encoding.UTF8.GetString(eventsSliceEvent.Event.Data));
                var eventType =
                    DomainEventTypes.FirstOrDefault(eventType => eventType.Name == eventsSliceEvent.Event.EventType);
                var eventData = new CustomEventData(orderId, metaData.CorrelationId,
                    metaData.TransactionId,
                    ((JObject) body.DomainEvent).ToObject(eventType));
                eventList.Add(eventData);
            }

            return eventList;
        }
        
        public async Task<IList<CustomEventData>> GetAllEventsCustomStream(string streamName)
        {
            var eventList = new List<CustomEventData> { };
            // var partitionId = Guid.Parse(Regex.Match(streamName, @"[{(]?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?").Value);
            var eventsSlice =
                await _connection.ReadStreamEventsForwardAsync(streamName, 0, 4096, true);
            foreach (var eventsSliceEvent in eventsSlice.Events)
            {
                var metaData =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<EventMetaData>(
                        Encoding.UTF8.GetString(eventsSliceEvent.Event.Metadata));
                var body = Newtonsoft.Json.JsonConvert.DeserializeObject<EventBody>(
                    Encoding.UTF8.GetString(eventsSliceEvent.Event.Data));
                var eventType =
                    DomainEventTypes.FirstOrDefault(eventType => eventType.Name == eventsSliceEvent.Event.EventType);
                var eventData = new CustomEventData(Guid.Empty, metaData.CorrelationId,
                    metaData.TransactionId,
                    ((JObject) body.DomainEvent).ToObject(eventType));
                eventList.Add(eventData);
            }

            return eventList;
        }

        public async Task PublishEvent(CustomEventData customEventData)
        {
            customEventData.EventMetaData.ApplicationId = _projectSettings.ApplicationId;
            var partitionId = _streamName + "-" + customEventData.EventMetaData.PartitionId;
            var result = await _connection.ConditionalAppendToStreamAsync(partitionId, ExpectedVersion.Any,
                new List<EventData>
                {
                    new EventData(
                        eventId: Guid.NewGuid(),
                        type: ((OrderBaseEvent) customEventData.EventBody.DomainEvent).GetEventName(),
                        isJson: true,
                        data: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(customEventData.EventBody)),
                        metadata: Encoding.UTF8.GetBytes(
                            JsonConvert.SerializeObject(customEventData.EventMetaData)))
                });
            switch (result.Status)
            {
                case ConditionalWriteStatus.Succeeded:
                    break;
                case ConditionalWriteStatus.VersionMismatch:
                    throw new Exception("VersionMismatch");
                case ConditionalWriteStatus.StreamDeleted:
                    throw new Exception("StreamDeleted");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<OrderAggregateRoot> GetAggregateRootSnapshot(Guid id)
        {
            return await _projectionAdapter.GetPartitonState<OrderAggregateRoot>(
                ORDER_PROJECTION_ORIGINAL_NAME, this.StreamName, id);
        }

        public async Task<OrderAggregateRoot> GetReadModelByProjection(Guid id)
        {
            return await _projectionAdapter.GetPartitonResult<OrderAggregateRoot>(ORDER_PROJECTION_ORIGINAL_NAME,
                this.StreamName,
                id);
        }

        public async Task AddSnapshotToDb(OrderAggregateRoot aggregateRoot)
        {
            var dbEntity = await _mongoRepository.GetAsync<OrderAggregateRoot>(o => o.Id == aggregateRoot.Id);
            if (dbEntity == default)
            {
                await _mongoRepository.InsertOneAsync<OrderAggregateRoot>(aggregateRoot);

                return;
            }

            await _mongoRepository.UpdateOneAsync<OrderAggregateRoot>(o => o.Id == aggregateRoot.Id, aggregateRoot);
        }
    }
}