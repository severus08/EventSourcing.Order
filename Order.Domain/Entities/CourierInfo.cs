using System;
using Order.Domain.Enums;
using Order.Domain.ValueObjects;

namespace Order.Domain.Entities
{
    public class CourierInfo
    {
        public Courier Courier { get; set; }
        public CourierTypeEnum CourierType { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public bool IsShipped { get; set; }
    }
}