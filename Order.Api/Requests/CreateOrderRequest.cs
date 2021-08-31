using System;
using System.Collections.Generic;

namespace Order.Api.Requests
{
    public class CreateOrderRequest : BaseRequest
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public string Seller { get; set; }
        public List<ProductDto> Products { get; set; }
        public bool DontRingBell { get; set; }
        public string AddressDetail { get; set; }
    }

    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}