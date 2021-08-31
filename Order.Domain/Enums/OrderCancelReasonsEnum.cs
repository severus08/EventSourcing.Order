using Order.Infrastructure.Enumaration;

namespace Order.Domain.Enums
{
    public class OrderCancelReasonsEnum : Enumaration
    {
        public static readonly OrderCancelReasonsEnum OutofStock = new OrderCancelReasonsEnum(1, "OutofStock");
        public static readonly OrderCancelReasonsEnum UnknownReason = new OrderCancelReasonsEnum(2, "UnknownReason");
        public OrderCancelReasonsEnum(int id, string name) : base(id, name)
        {
        }
        public static OrderCancelReasonsEnum Create(int id)
        {
            return id switch
            {
                1 => OrderCancelReasonsEnum.OutofStock,
                _ => OrderCancelReasonsEnum.UnknownReason
            };
        }
        public OrderCancelReasonsEnum() { }
    }
}