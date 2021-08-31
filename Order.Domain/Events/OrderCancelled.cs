using System;
using Order.Domain.AggregateRoot;
using Order.Domain.Enums;
using Order.Infrastructure.Attribute;

namespace Order.Domain.Events
{
    [EventName("OrderCancelled")]

    public class OrderCancelled : OrderBaseEvent
    {
        public DateTime CancelledAt { get; set; }
        public int CancelReasonId { get; set; }
        public override void Apply(OrderAggregateRoot aggregateRoot)
        {
            aggregateRoot.Status = OrderStatusEnum.Cancelled;
            aggregateRoot.StatusHistory.Add(new OrderStatusEnum(OrderStatusEnum.Cancelled.Id,OrderStatusEnum.Cancelled.Name,CancelledAt));
            aggregateRoot.CancelReason = OrderCancelReasonsEnum.Create(CancelReasonId);
        }
    }
}