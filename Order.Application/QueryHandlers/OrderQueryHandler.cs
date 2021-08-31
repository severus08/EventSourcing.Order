using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Order.Application.Queries;
using Order.Domain.DomainSpecifications;
using Order.Domain.ReadModels;
using Order.Infrastructure.Query;
using Order.Infrastructure.ReadModel;

namespace Order.Application.QueryHandlers
{
    public class OrderQueryHandler : IQueryHandler<GetOrderQuery, OrderReadModel>
        , IQueryHandler<GetOrderListQuery, List<OrderReadModel>>
    {
        private readonly IDomainReadModelRepository<OrderReadModel> _domainReadModelRepository;

        public OrderQueryHandler(IDomainReadModelRepository<OrderReadModel> domainReadModelRepository)
        {
            _domainReadModelRepository = domainReadModelRepository;
        }

        public async Task<OrderReadModel> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var order = (await _domainReadModelRepository.Filter(x => x.OrderId == request.OrderId)).FirstOrDefault();
            if (order == default)
            {
                return default;
            }

            if (request.SpecificationList != default && request.SpecificationList.Any() &&
                request.SpecificationList.Any(x => !order.Evaluate(x)))
            {
                return default;
            }

            return order;
        }

        public async Task<List<OrderReadModel>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
        {
            if (request.SpecificationList == default || !request.SpecificationList.Any())
            {
                return default;
            }

            var orders = await _domainReadModelRepository.DomainFilter(request.SpecificationList);

            return orders;
        }
    }
}