using System;
using System.Threading.Tasks;
using Order.Infrastructure.Domain;

namespace Order.Infrastructure.Factories
{
    public interface IDomainFactory<T> where T:IAggregateRoot
    {
        public T Instantiate(Guid id,Guid transactionId);
        public Task<T> GetLastState(Guid id);
        Task<T> GetReadModel(Guid id);
    }
}