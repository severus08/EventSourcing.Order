using System.Threading.Tasks;

namespace Order.Infrastructure.Events
{
    public interface IEventBus
    {
        Task Publish(params IEvent[] events);
    }
}