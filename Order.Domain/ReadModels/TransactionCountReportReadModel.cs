using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Order.Domain.ReadModels
{
    public class TransactionCountReportReadModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public int TotalOrderCount { get; set; }
        public int CancelledOrderCount { get; set; }
        public int PaymentTransactionCount { get; set; }
        public int OnDeliveryOrderCount { get; set; }
        public int DeliveredOrderCount { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}