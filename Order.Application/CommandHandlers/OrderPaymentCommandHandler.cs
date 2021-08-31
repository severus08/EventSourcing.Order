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
    public class OrderPaymentCommandHandler : ICommandHandler<PaymentApproveCommand>
        , ICommandHandler<PaymentRejectCommand>
    {
        private readonly IDomainFactory<OrderAggregateRoot> _domainFactory;
        private readonly IMapper _mapper;

        public OrderPaymentCommandHandler(IDomainFactory<OrderAggregateRoot> domainFactory, IMapper mapper)
        {
            _domainFactory = domainFactory;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(PaymentApproveCommand request, CancellationToken cancellationToken)
        {
            var domain = _domainFactory.Instantiate(request.OrderId, request.TransactionId);
            await domain.PaymentApprove(_mapper.Map<PaymentApproveDto>(request));
            return Unit.Value;
        }

        public async Task<Unit> Handle(PaymentRejectCommand request, CancellationToken cancellationToken)
        {
            var domain = _domainFactory.Instantiate(request.OrderId, request.TransactionId);
            await domain.PaymentRejected(_mapper.Map<PaymentRejectDto>(request));
            return Unit.Value;
        }
    }
}