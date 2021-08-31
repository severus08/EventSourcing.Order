using Order.Domain.AggregateRoot;
using Order.Infrastructure.Attribute;
using Order.Infrastructure.Domain;

namespace Order.Domain.Events
{
    public abstract class OrderBaseEvent : BaseEvent<OrderAggregateRoot>
    {
        
    }
    public static class EventExtensions
    {
        public static string GetEventName(this OrderBaseEvent orderBaseEvent)
        {
            return ((EventNameAttribute)EventNameAttribute.GetCustomAttribute(orderBaseEvent.GetType(), typeof(EventNameAttribute))).EventName;
        }
    }
}