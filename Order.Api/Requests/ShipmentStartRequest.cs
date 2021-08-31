using System;

namespace Order.Api.Requests
{
    public class ShipmentStartRequest : BaseRequest
    {
        public Guid CourierId { get; set; }
        public string CourierName { get; set; }
        public int CourierTypeId { get; set; }
    }
}