using System;
using Order.Infrastructure.Enumaration;

namespace Order.Domain.Enums
{
    public class OrderStatusEnum : Enumaration
    {
        public static readonly OrderStatusEnum Initialized = new OrderStatusEnum(1, "Order Initialized");
        public static readonly OrderStatusEnum Suspended = new OrderStatusEnum(2, "Order Suspended");
        public static readonly OrderStatusEnum Completed = new OrderStatusEnum(3, "Order Completed");
        public static readonly OrderStatusEnum Cancelled = new OrderStatusEnum(4, "Order Cancelled");
        public DateTime ProcessDate { get; set; }
        public OrderStatusEnum(int id, string name,DateTime processDate) : base(id, name)
        {
            ProcessDate = processDate;
        }
        public OrderStatusEnum(int id, string name) : base(id, name)
        {
        }
        public OrderStatusEnum() { }
    }
}