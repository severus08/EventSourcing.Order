using System;
using System.Linq.Expressions;

namespace Order.Infrastructure.ReadModel
{
    public abstract class Specification<T>
    {
        public abstract Expression<Func<T, bool>> Expression();

        public bool IsSatisfiedBy(T entity)
        {
            Func<T, bool> predicate = Expression().Compile();

            return predicate(entity);
        }
    }
}