using System;
using Order.Domain.Enums;

namespace Order.Domain.ValueObjects
{
    public class PaymentInfo
    {
        public Guid Id { get; set; }
        public PaymentRejectReasonsEnum Reason { get; set; }
    }
}