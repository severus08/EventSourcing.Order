using System;
using System.Collections.Generic;
using Order.Domain.Entities;

namespace Order.Application.Commands
{
    public class CreateOrderCommand  : OrderBaseCommand
    {
        public Guid CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public string Seller { get; set; }
        public List<Product> Products { get; set; }
        public bool DontRingBell { get; set; }
        public string AddressDetail { get; set; }
    }
}