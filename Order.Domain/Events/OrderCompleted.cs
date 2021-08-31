using System;
using Order.Domain.AggregateRoot;
using Order.Domain.Enums;
using Order.Infrastructure.Attribute;
using Order.Infrastructure.EventStore;

namespace Order.Domain.Events
{
    [EventName("OrderCompleted")]
    public class OrderCompleted : OrderBaseEvent
    {
        public DateTime CompletedAt { get; set; }
        public override void Apply(OrderAggregateRoot aggregateRoot)
        {
            aggregateRoot.Status = OrderStatusEnum.Completed;
            aggregateRoot.StatusHistory.Add(new OrderStatusEnum(OrderStatusEnum.Completed.Id,OrderStatusEnum.Completed.Name,CompletedAt));
        }
    }
}