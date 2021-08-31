using System;
using System.Collections.Generic;
using Order.Domain.Entities;

namespace Order.Domain.Dto
{
    public class CreateOrderDto : OrderBaseDto
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string Seller { get; set; }
        public string CustomerFullName { get; set; }
        public List<Product> Products { get; set; }
        public bool DontRingBell { get; set; }
        public string AddressDetail { get; set; }
    }
}