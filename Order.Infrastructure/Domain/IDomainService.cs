using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Order.Infrastructure.EventStore;

namespace Order.Infrastructure.Domain
{
    public interface IDomainService<T> where T:IAggregateRoot
    {
        Task<IList<CustomEventData>> GetAllEvent(T aggregateRoot);
        Task PublishEvent(CustomEventData customEventData);
        public Task<T> GetAggregateRootSnapshot(Guid id);
        public Task<T> GetReadModelByProjection(Guid id);
    }
}
