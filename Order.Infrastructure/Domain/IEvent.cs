namespace Order.Infrastructure.Domain
{
    public interface IEvent<T> where T : IAggregateRoot
    {
        void Apply(T aggregateRoot);
    }
}