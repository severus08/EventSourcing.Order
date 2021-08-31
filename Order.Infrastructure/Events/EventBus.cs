using System;
using System.Threading.Tasks;
using MediatR;


namespace Order.Infrastructure.Events
{
    public class EventBus: IEventBus
    {
        private readonly IMediator _mediator;

        public EventBus(
            IMediator mediator
        )
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Publish(params IEvent[] events)
        {
            foreach (var @event in events)
            {
                await _mediator.Publish(@event);
            }
        }
    }
}
