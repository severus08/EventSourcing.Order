namespace Order.Infrastructure.Domain
{
    public abstract class BaseEvent<T> : IEvent<T> where T : IAggregateRoot
    {
        public abstract void Apply(T aggregateRoot);
    }
}