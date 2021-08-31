using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Order.Domain.AggregateRoot;
using Order.Infrastructure.AppSettingsConfigration;
using Order.Infrastructure.Domain;
using Order.Infrastructure.Factories;

namespace Order.Domain.Factory
{
    public class OrderFactory : IDomainFactory<OrderAggregateRoot>
    {
        private readonly IDomainService<OrderAggregateRoot> _domainService;
        private readonly ProjectSettings _projectSettings;


        public OrderFactory(IDomainService<OrderAggregateRoot> domainService,
            IOptions<ProjectSettings> option)
        {
            _domainService = domainService;
            _projectSettings = option.Value;
        }

        /// <summary>
        /// get last state 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<OrderAggregateRoot> GetLastState(Guid id)
        {
            switch (_projectSettings.AggregateRootCreationType)
            {
                case ModelCreationTypeEnum.Client:
                    return await Task.Run(() =>
                        new OrderAggregateRoot(id, _domainService)
                    );
                case ModelCreationTypeEnum.Server:
                    return await _domainService.GetAggregateRootSnapshot(id);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// get read model with eventStoreResult
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<OrderAggregateRoot> GetReadModel(Guid id)
        {
            switch (_projectSettings.ReadModelCreationType)
            {
                case ModelCreationTypeEnum.Client:
                    break;
                case ModelCreationTypeEnum.Server:
                    return await _domainService.GetReadModelByProjection(id);
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return await _domainService.GetReadModelByProjection(id);
        }

        /// <summary>
        /// get last state with apply methods
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public OrderAggregateRoot Instantiate(Guid id, Guid transactionId)
        {
            return new OrderAggregateRoot(id, _domainService, transactionId);
        }
    }
}