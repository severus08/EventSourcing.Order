// using System;
// using System.Collections.Generic;
// using System.Linq.Expressions;
// using System.Threading.Tasks;
// using Order.Domain.DomainSpecifications;
//
// namespace Order.Domain.Repositories
// {
//     public interface IDomainReadModelRepository<T>
//     {
//         Task Upsert(T entity);
//         Task<List<T>> DomainFilter(Specification<T> filter);
//         Task<List<T>> Filter(Expression<Func<T, bool>> expression);
//         Task<T> Get(Expression<Func<T, bool>> expression);
//     }
// }