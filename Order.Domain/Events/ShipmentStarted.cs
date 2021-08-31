using System;
using Order.Domain.AggregateRoot;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.ValueObjects;
using Order.Infrastructure.Attribute;

namespace Order.Domain.Events
{
    [EventName("ShipmentStarted")]
    public class ShipmentStarted : OrderBaseEvent
    {
        public Guid CourierId { get; set; }
        public string CourierName { get; set; }
        public int CourierTypeId { get; set; }
        public DateTime StartedAt { get; set; }
        public override void Apply(OrderAggregateRoot aggregateRoot)
        {
            aggregateRoot.CourierInfo = new CourierInfo
            {
                Courier = new Courier
                {
                    Id = CourierId,
                    CourierName = CourierName
                },
                CourierType = CourierTypeEnum.Create(CourierTypeId),
                StartedAt = StartedAt
            };
        }
    }
}