using System;
using System.Windows.Input;

namespace Order.Application.Commands
{
    public class OrderBaseCommand : Order.Infrastructure.Command.ICommand
    {
        public Guid OrderId { get; set; }
        public DateTime ProcessDate { get; set; } = DateTime.Now;
        public Guid TransactionId { get; set; }
        public Guid CorrelationId { get; set; }
    }
}