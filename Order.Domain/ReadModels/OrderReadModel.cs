using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Order.Domain.AggregateRoot;
using Order.Domain.DomainSpecifications;
using Order.Domain.Entities;
using Order.Infrastructure.Domain;
using Order.Infrastructure.ReadModel;

namespace Order.Domain.ReadModels
{
    public class OrderReadModel : IDomainReadModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public Guid OrderId { get; set; }
        public OrderStatusDto OrderStatus { get; set; }
        public List<OrderStatusDto> OrderStatusHistory { get; set; }
        public OrderReasonDto CancelReason { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public bool DontRingBell { get; set; }
        public string AddressDetail { get; set; }
        public List<Product> Products { get; set; }
        public Guid? PaymentTransactionId { get; set; }
        public OrderReasonDto PaymentCancelReason { get; set; }
        public Guid? CourierId { get; set; }
        public string CourierName { get; set; }
        public string CourierTypeDescription { get; set; }
        public bool IsShipmentFinished { get; set; }
        public bool IsPaid { get; set; }

        public static OrderReadModel Create(OrderAggregateRoot aggregateRoot)
            => new OrderReadModel
            {
                OrderId = aggregateRoot.Id,
                OrderStatus = new OrderStatusDto
                {
                    Id = aggregateRoot.Status.Id,
                    Description = aggregateRoot.Status.Name
                },
                CancelReason = aggregateRoot.CancelReason != null
                    ? new OrderReasonDto
                    {
                        Id = aggregateRoot.CancelReason.Id,
                        Description = aggregateRoot.CancelReason.Name
                    }
                    : null,
                CustomerId = aggregateRoot.ReceiverInfo.Customer.Id,
                CustomerName = aggregateRoot.ReceiverInfo.Customer.FullName,
                DontRingBell = aggregateRoot.ReceiverInfo.DontRingBell,
                AddressDetail = aggregateRoot.ReceiverInfo.AddressDetail,
                Products = aggregateRoot.Products,
                PaymentTransactionId = aggregateRoot.PaymentInfo.Id,
                PaymentCancelReason = aggregateRoot.PaymentInfo.Reason != null
                    ? new OrderReasonDto
                    {
                        Id = aggregateRoot.PaymentInfo.Reason.Id,
                        Description = aggregateRoot.PaymentInfo.Reason.Name
                    }
                    : null,
                CourierId = aggregateRoot.CourierInfo.Courier.Id,
                CourierName = aggregateRoot.CourierInfo.Courier.CourierName,
                CourierTypeDescription = aggregateRoot.CourierInfo.CourierType.Name,
                IsShipmentFinished = aggregateRoot.CourierInfo.IsShipped,
                IsPaid = aggregateRoot.IsPaid
            };
        
        

        public bool Evaluate(Specification<OrderReadModel> specification)
        {
            return specification.IsSatisfiedBy(this);
        }
    }

    #region Dto

    public class OrderStatusDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class OrderReasonDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    #endregion
}