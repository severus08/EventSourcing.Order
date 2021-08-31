using System;

namespace Order.Domain.Entities
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}