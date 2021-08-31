using MediatR;

namespace Order.Infrastructure.Events
{
    public interface IEventHandler<in TEvent>: INotificationHandler<TEvent>
           where TEvent : IEvent
    {
    }
}
