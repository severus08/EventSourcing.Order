using System;

namespace Order.Api.Requests
{
    public class PaymentApproveRequest : BaseRequest
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
    }
}