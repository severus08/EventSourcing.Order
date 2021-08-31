using System;

namespace Order.Application.Commands
{
    public class ShipmentStartCommand  : OrderBaseCommand
    {
        public Guid CourierId { get; set; }
        public string CourierName { get; set; }
        public int CourierTypeId { get; set; }
    }
}