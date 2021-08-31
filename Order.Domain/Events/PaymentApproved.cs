using System;
using Order.Domain.AggregateRoot;
using Order.Domain.ValueObjects;
using Order.Infrastructure.Attribute;

namespace Order.Domain.Events
{
    [EventName("PaymentApproved")]
    public class PaymentApproved : OrderBaseEvent
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public override void Apply(OrderAggregateRoot aggregateRoot)
        {
            aggregateRoot.PaymentInfo = new PaymentInfo
            {
                Id = PaymentId,
            };
            aggregateRoot.IsPaid = true;
        }
    }
}