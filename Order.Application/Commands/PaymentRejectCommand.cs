using System;

namespace Order.Application.Commands
{
    public class PaymentRejectCommand  : OrderBaseCommand
    {
        public Guid PaymentId { get; set; }
        public int ReasonId { get; set; }
    }
}