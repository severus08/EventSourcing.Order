using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Order.Application.Commands;
using Order.Domain.AggregateRoot;
using Order.Domain.Dto;
using Order.Infrastructure.Command;
using Order.Infrastructure.Factories;

namespace Order.Application.CommandHandlers
{
    public class OrderCommandHandler : ICommandHandler<CreateOrderCommand>
    ,ICommandHandler<CancelOrderCommand>
    {
        private readonly IDomainFactory<OrderAggregateRoot> _domainFactory;
        private readonly IMapper _mapper;

        public OrderCommandHandler(IDomainFactory<OrderAggregateRoot> domainFactory, IMapper mapper)
        {
            _domainFactory = domainFactory;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var domain = _domainFactory.Instantiate(request.OrderId, request.TransactionId);
            await domain.CreateOrder(_mapper.Map<CreateOrderDto>(request));
            return Unit.Value;
        }

        public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var domain = _domainFactory.Instantiate(request.OrderId, request.TransactionId);
            await domain.CancelOrder(_mapper.Map<CancelOrderDto>(request));
            return Unit.Value;
        }
    }
}