using System;
using Order.Domain.Enums;

namespace Order.Domain.ValueObjects
{
    public class Courier
    {
        public Guid Id { get; set; }
        public string CourierName { get; set; }
    }
}