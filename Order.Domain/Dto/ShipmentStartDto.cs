using System;

namespace Order.Domain.Dto
{
    public class ShipmentStartDto : OrderBaseDto
    {
        public Guid CourierId { get; set; }
        public string CourierName { get; set; }
        public int CourierTypeId { get; set; }
    }
}