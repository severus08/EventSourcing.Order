using System;
using System.Collections.Generic;
using Order.Domain.AggregateRoot;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.ValueObjects;
using Order.Infrastructure.Attribute;

namespace Order.Domain.Events
{
    [EventName("OrderCreated")]
    public class OrderCreated : OrderBaseEvent
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public List<Product> Products { get; set; }
        public bool DontRingBell { get; set; }
        public string AddressDetail { get; set; }

        public DateTime CreatedAt { get; set; }

        public override void Apply(OrderAggregateRoot aggregateRoot)
        {
            aggregateRoot.Id = Id;
            aggregateRoot.ReceiverInfo = new ReceiverInfo
            {
                Customer = new Customer
                {
                    Id = CustomerId,
                    FullName = CustomerFullName
                },
                DontRingBell = DontRingBell, AddressDetail = AddressDetail
            };
            aggregateRoot.Status = OrderStatusEnum.Initialized;
            aggregateRoot.StatusHistory.Add(new OrderStatusEnum(OrderStatusEnum.Initialized.Id,
                OrderStatusEnum.Initialized.Name, CreatedAt));
            aggregateRoot.Products.AddRange(Products);
        }
    }
}