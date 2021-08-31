using System;
using Order.Domain.AggregateRoot;
using Order.Infrastructure.Attribute;

namespace Order.Domain.Events
{
    [EventName("ShipmentFinished")]
    public class ShipmentFinished : OrderBaseEvent
    {
        public DateTime FinishedAt { get; set; }
        public override void Apply(OrderAggregateRoot aggregateRoot)
        {
            aggregateRoot.CourierInfo.FinishedAt = FinishedAt;
            aggregateRoot.CourierInfo.IsShipped = true;
        }
    }
}