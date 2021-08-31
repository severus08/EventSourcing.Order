using System;

namespace Order.Application.Commands
{
    public class PaymentApproveCommand  : OrderBaseCommand
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
    }
}