using System;

namespace Order.Domain.Dto
{
    public class PaymentApproveDto : OrderBaseDto
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
    }
}