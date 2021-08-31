using System;
using System.Collections.Generic;
using Order.Domain.ReadModels;
using Order.Infrastructure.Query;
using Order.Infrastructure.ReadModel;

namespace Order.Application.Queries
{
    public class GetOrderQuery : Order.Infrastructure.Command.ICommand, IQuery<OrderReadModel>
    {
        public Guid OrderId { get; set; }
        public List<Specification<OrderReadModel>> SpecificationList;
    }
}