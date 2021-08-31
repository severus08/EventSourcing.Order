using System;

namespace Order.Domain.Dto
{
    public class PaymentRejectDto : OrderBaseDto
    {
        public Guid PaymentId { get; set; }
        public int ReasonId { get; set; }
    }
}