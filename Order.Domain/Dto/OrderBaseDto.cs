using System;

namespace Order.Domain.Dto
{
    public class OrderBaseDto
    {
        public DateTime ProcessDate { get; set; } = DateTime.Now;
        public Guid TransactionId { get; set; }
        public Guid CorrelationId { get; set; }
    }
}