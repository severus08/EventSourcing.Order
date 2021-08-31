using System.Collections.Generic;
using Order.Domain.ReadModels;
using Order.Infrastructure.Query;
using Order.Infrastructure.ReadModel;

namespace Order.Application.Queries
{
    public class GetOrderListQuery : Order.Infrastructure.Command.ICommand, IQuery<List<OrderReadModel>>
    {
        public List<Specification<OrderReadModel>> SpecificationList;
    }
}