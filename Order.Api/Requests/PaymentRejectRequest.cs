using System;

namespace Order.Api.Requests
{
    public class PaymentRejectRequest : BaseRequest
    {
        public Guid PaymentId { get; set; }
        public int ReasonId { get; set; }
    }
}