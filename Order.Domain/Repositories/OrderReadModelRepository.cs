using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Xml.Schema;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Order.Domain.DomainSpecifications;
using Order.Domain.ReadModels;
using Order.Infrastructure.Extensions;
using Order.Infrastructure.Mongo;
using Order.Infrastructure.ReadModel;

namespace Order.Domain.Repositories
{
    public class OrderReadModelRepository : IDomainReadModelRepository<OrderReadModel>
    {
        private readonly IMongoRepository _mongoRepository;

        public OrderReadModelRepository(IMongoRepository mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        public async Task Upsert(OrderReadModel entity)
        {
            var dbEntity = await _mongoRepository.GetAsync<OrderReadModel>(o => o.OrderId == entity.OrderId);
            if (dbEntity == default)
            {
                await _mongoRepository.InsertOneAsync<OrderReadModel>(entity);

                return;
            }
            entity.Id = dbEntity.Id;
            await _mongoRepository.UpdateOneAsync<OrderReadModel>(o => o.Id == entity.Id, entity);
        }

        public async Task<List<OrderReadModel>> DomainFilter(Specification<OrderReadModel> filter)
        {            
            return await _mongoRepository.SearchAsync(filter.Expression());
        }
        public async Task<List<OrderReadModel>> DomainFilter(List<Specification<OrderReadModel>> filters)
        {
            if (filters == default || !filters.Any())
            {
                throw new Exception("filters is null!");
            }
            if (filters.Count == 1)
            {
                return await DomainFilter(filters.FirstOrDefault());
            }
            var filterDefinition = Builders<OrderReadModel>.Filter.And(filters[0].Expression(),filters[1].Expression());

            for (int i = 1; i < filters.Count; i++)
            {
                filterDefinition = Builders<OrderReadModel>.Filter.And(filters[i - 1].Expression(), filters[i].Expression());
            }
            
            return await _mongoRepository.SearchAsync<OrderReadModel>(x => filterDefinition.Inject());
        }
        public async Task<List<OrderReadModel>> Filter(Expression<Func<OrderReadModel, bool>> expression)
        {
            return await _mongoRepository.SearchAsync<OrderReadModel>(expression);
        }

        public async Task<OrderReadModel> Get(Expression<Func<OrderReadModel, bool>> expression)
        {
            return await _mongoRepository.GetAsync(expression);
        }
    }
}