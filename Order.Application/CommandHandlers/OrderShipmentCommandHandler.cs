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
    public class OrderShipmentCommandHandler : ICommandHandler<ShipmentStartCommand>
        , ICommandHandler<ShipmentFinishCommand>
    {
        private readonly IDomainFactory<OrderAggregateRoot> _domainFactory;
        private readonly IMapper _mapper;

        public OrderShipmentCommandHandler(IDomainFactory<OrderAggregateRoot> domainFactory, IMapper mapper)
        {
            _domainFactory = domainFactory;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(ShipmentStartCommand request, CancellationToken cancellationToken)
        {
            var domain = _domainFactory.Instantiate(request.OrderId, request.TransactionId);
            await domain.ShipmentStart(_mapper.Map<ShipmentStartDto>(request));
            return Unit.Value;
        }

        public async Task<Unit> Handle(ShipmentFinishCommand request, CancellationToken cancellationToken)
        {
            var domain = _domainFactory.Instantiate(request.OrderId, request.TransactionId);
            await domain.ShipmentFinish(_mapper.Map<ShipmentFinishDto>(request));
            return Unit.Value;
        }
    }
}