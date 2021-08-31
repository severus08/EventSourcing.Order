using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Order.Domain.AggregateRoot;
using Order.Infrastructure.Domain;
using Order.Infrastructure.EventStore;

namespace Order.Domain
{
    public class OrderDomainService : IDomainService<OrderAggregateRoot>
    {
        private readonly IEventRepository<OrderAggregateRoot> _eventRepository;

        public OrderDomainService(IEventRepository<OrderAggregateRoot> eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IList<CustomEventData>> GetAllEvent(OrderAggregateRoot aggregateRoot)
        {
            return await _eventRepository.GetAllEvents(aggregateRoot.Id);
        }

        public async Task PublishEvent(CustomEventData customEventData)
        {
            await _eventRepository.PublishEvent(customEventData);
        }

        public async Task<OrderAggregateRoot> GetAggregateRootSnapshot(Guid id)
        {
            return await _eventRepository.GetAggregateRootSnapshot(id); 
        }

        public async Task<OrderAggregateRoot> GetReadModelByProjection(Guid id)
        {
            return await _eventRepository.GetReadModelByProjection(id);
        }
    }
}