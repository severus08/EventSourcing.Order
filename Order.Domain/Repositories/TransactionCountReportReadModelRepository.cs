// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Linq.Expressions;
// using System.Threading.Tasks;
// using MongoDB.Driver;
// using MongoDB.Driver.Linq;
// using Order.Domain.ReadModels;
// using Order.Infrastructure.Mongo;
// using Order.Infrastructure.ReadModel;
//
// namespace Order.Domain.Repositories
// {
//     public class TransactionCountReportReadModelRepository : IDomainReadModelRepository<TransactionCountReportReadModel>
//     {
//         private readonly IMongoRepository _mongoRepository;
//
//         public TransactionCountReportReadModelRepository(IMongoRepository mongoRepository)
//         {
//             _mongoRepository = mongoRepository;
//         }
//
//         public async Task Upsert(TransactionCountReportReadModel entity)
//         {
//             await _mongoRepository.InsertOneAsync<TransactionCountReportReadModel>(entity);
//         }
//
//         public async Task<List<TransactionCountReportReadModel>> DomainFilter(Specification<TransactionCountReportReadModel> filter)
//         {            
//             return await _mongoRepository.SearchAsync(filter.Expression());
//         }
//         
//         public async Task<List<TransactionCountReportReadModel>> DomainFilter(List<Specification<TransactionCountReportReadModel>> filters)
//         {
//             if (filters == default || !filters.Any())
//             {
//                 throw new Exception("filters is null!");
//             }
//             if (filters.Count == 1)
//             {
//                 return await DomainFilter(filters.FirstOrDefault());
//             }
//             var filterDefinition = Builders<TransactionCountReportReadModel>.Filter.And(filters[0].Expression(),filters[1].Expression());
//
//             for (int i = 2; i < filters.Count; i++)
//             {
//                 filterDefinition = Builders<TransactionCountReportReadModel>.Filter.And(filters[i - 1].Expression(), filters[i].Expression());
//             }
//             
//             return await _mongoRepository.SearchAsync<TransactionCountReportReadModel>(x => filterDefinition.Inject());
//         }
//         
//         public async Task<List<TransactionCountReportReadModel>> Filter(Expression<Func<TransactionCountReportReadModel, bool>> expression)
//         {
//             return await _mongoRepository.SearchAsync<TransactionCountReportReadModel>(expression);
//         }
//
//         public async Task<TransactionCountReportReadModel> Get(Expression<Func<TransactionCountReportReadModel, bool>> expression)
//         {
//             return await _mongoRepository.GetAsync(expression);
//         }
//     }
// }