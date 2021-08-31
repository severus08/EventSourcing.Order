using System;
using Order.Domain.AggregateRoot;
using Order.Domain.Enums;
using Order.Domain.ValueObjects;
using Order.Infrastructure.Attribute;

namespace Order.Domain.Events
{
    [EventName("PaymentRejected")]
    public class PaymentRejected : OrderBaseEvent
    {
        public Guid PaymentId { get; set; }
        public int ReasonId { get; set; }
        public DateTime RejectedAt { get; set; }
        public override void Apply(OrderAggregateRoot aggregateRoot)
        {
            aggregateRoot.Status = OrderStatusEnum.Suspended;
            aggregateRoot.StatusHistory.Add(new OrderStatusEnum(OrderStatusEnum.Suspended.Id,OrderStatusEnum.Suspended.Name,RejectedAt));
            aggregateRoot.IsPaid = false;
            aggregateRoot.PaymentInfo = new PaymentInfo
            {
                Id = PaymentId,
                Reason = PaymentRejectReasonsEnum.Create(ReasonId)
            };
        }
    }
}