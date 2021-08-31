using System;
using System.Text.Json.Serialization;
using Order.Infrastructure.Domain;
using Newtonsoft.Json;

namespace Order.Infrastructure.EventStore
{
    public class CustomEventData
    {
        public EventMetaData EventMetaData { get; set; }
        public EventBody EventBody { get; set; }

        public CustomEventData(Guid partitionId, string correlationId, string transactionId, object baseEvent)
        {
            this.EventMetaData = new EventMetaData
            {
                CorrelationId = correlationId,
                TransactionId = transactionId,
                PartitionId = partitionId
            };
            this.EventBody = new EventBody
            {
                DomainEvent = baseEvent
            };
        }
    }

    public class EventMetaData
    {
        public Guid ApplicationId { get; set; }
        [JsonProperty("$correlationId")] public string CorrelationId { get; set; }
        public string TransactionId { get; set; }

        public Guid PartitionId { get; set; }
    }

    public class EventBody
    {
        public object DomainEvent { get; set; }
    }
}