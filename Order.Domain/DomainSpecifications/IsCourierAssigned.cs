using System;
using System.Linq.Expressions;
using Order.Domain.AggregateRoot;
using Order.Domain.ReadModels;
using Order.Infrastructure.ReadModel;

namespace Order.Domain.DomainSpecifications
{

    public class IsCourierAssigned : Specification<OrderReadModel>
    {
        public override Expression<Func<OrderReadModel, bool>> Expression()
             => agg => agg.CourierId != null;
    }
}