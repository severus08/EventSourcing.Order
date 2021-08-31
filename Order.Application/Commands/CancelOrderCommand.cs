using System;

namespace Order.Application.Commands
{
    public class CancelOrderCommand : OrderBaseCommand
    {
        public int CancelReasonId { get; set; }
    }
}