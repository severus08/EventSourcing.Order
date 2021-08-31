using System;

namespace Order.Domain.ValueObjects
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
    }
}