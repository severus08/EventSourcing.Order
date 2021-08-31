using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Order.Infrastructure.Domain;

namespace Order.Infrastructure.EventStore
{
    public interface
        IEventRepository<T> where T : IAggregateRoot
    {
        Task<IList<CustomEventData>> GetAllEvents(Guid orderId);

        Task PublishEvent(CustomEventData customEventData);
        public Task<T> GetAggregateRootSnapshot(Guid id);
        public Task<T> GetReadModelByProjection(Guid id);
        public Task AddSnapshotToDb(T aggregateRoot);
        Task<IList<CustomEventData>> GetAllEventsCustomStream(string streamName);
    }
}