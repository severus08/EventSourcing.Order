using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Order.Domain.DomainSpecifications;
using Order.Domain.Dto;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.Events;
using Order.Domain.ValueObjects;
using Order.Infrastructure;
using Order.Infrastructure.Attribute;
using Order.Infrastructure.Domain;
using Order.Infrastructure.EventStore;

namespace Order.Domain.AggregateRoot
{
    [StreamName(Constants.ORDER_STREAM_NAME)]
    public class OrderAggregateRoot : IAggregateRoot
    {
        #region Properties

        public Guid Id { get; set; }
        public OrderStatusEnum Status { get; set; }
        public List<OrderStatusEnum> StatusHistory { get; set; } = new();
        public OrderCancelReasonsEnum CancelReason { get; set; }
        public ReceiverInfo ReceiverInfo { get; set; }

        public List<Product> Products { get; set; } = new();
        public PaymentInfo PaymentInfo { get; set; }
        public CourierInfo CourierInfo { get; set; }
        public bool IsPaid { get; set; }
        public int Version { get; set; } = -1;

        #endregion

        private readonly IDomainService<OrderAggregateRoot> _domainService;

        #region ctor

        /// <summary>
        /// for mapper
        /// </summary>
        public OrderAggregateRoot()
        {
        }

        public OrderAggregateRoot(Guid orderId, IDomainService<OrderAggregateRoot> domainService)
        {
            _domainService = domainService;
            Id = orderId;

            var events = (_domainService.GetAllEvent(this).GetAwaiter().GetResult()).ToList();
            events.ForEach((x) =>
                {
                    ((IEvent<OrderAggregateRoot>) x.EventBody.DomainEvent).Apply(this);
                    Version++;
                }
            );
        }

        /// <summary>
        /// for commandHandlers
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="domainService"></param>
        /// <param name="transactionId"></param>
        /// <exception cref="Exception"></exception>
        public OrderAggregateRoot(Guid orderId, IDomainService<OrderAggregateRoot> domainService,
            Guid transactionId)
        {
            _domainService = domainService;
            Id = orderId;

            var events = (_domainService.GetAllEvent(this).GetAwaiter().GetResult()).ToList();
            events.ForEach((x) =>
                {
                    ((OrderBaseEvent) x.EventBody.DomainEvent).Apply(this);
                    Version++;
                }
            );

            if (events.Any(@event => @event.EventMetaData.TransactionId == transactionId.ToString()))
            {
                // TODO : customize this exceptionType
                throw new Exception("Idempotent Command!");
            }
        }

        #endregion

        public async Task CreateOrder(CreateOrderDto createOrder)
        {
            //third-party-calls or some business
            await _domainService.PublishEvent(new CustomEventData(createOrder.OrderId, createOrder.CorrelationId.ToString(), createOrder.TransactionId.ToString(), new OrderCreated
            {
                Id = createOrder.OrderId,
                CustomerId = createOrder.CustomerId,
                Products = createOrder.Products,
                AddressDetail = createOrder.AddressDetail,
                CustomerFullName = createOrder.CustomerFullName,
                DontRingBell = createOrder.DontRingBell,
                CreatedAt = createOrder.ProcessDate
            }));
        }
        public async Task CancelOrder(CancelOrderDto cancelOrder)
        {
            //third-party-calls or some business
            await _domainService.PublishEvent(new CustomEventData(Id, cancelOrder.CorrelationId.ToString(), cancelOrder.TransactionId.ToString(), new OrderCancelled
            {
                CancelReasonId = cancelOrder.CancelReasonId,
                CancelledAt = cancelOrder.ProcessDate
            }));
        }
        public async Task PaymentApprove(PaymentApproveDto paymentApprove)
        {
            //third-party-calls or some business
            await _domainService.PublishEvent(new CustomEventData(Id, paymentApprove.CorrelationId.ToString(), paymentApprove.TransactionId.ToString(), new PaymentApproved
            {
                PaymentId = paymentApprove.PaymentId,
                Amount = paymentApprove.Amount
            }));
        }
        public async Task PaymentRejected(PaymentRejectDto paymentReject)
        {
            //third-party-calls or some business
            await _domainService.PublishEvent(new CustomEventData(Id, paymentReject.CorrelationId.ToString(), paymentReject.TransactionId.ToString(), new PaymentRejected
            {
                PaymentId = paymentReject.PaymentId,
                ReasonId = paymentReject.ReasonId,
                RejectedAt = paymentReject.ProcessDate
            }));
        }
        public async Task ShipmentStart(ShipmentStartDto shipmentStart)
        {
            //third-party-calls or some business
            await _domainService.PublishEvent(new CustomEventData(Id, shipmentStart.CorrelationId.ToString(), shipmentStart.TransactionId.ToString(), new ShipmentStarted
            {
                CourierId = shipmentStart.CourierId,
                CourierName = shipmentStart.CourierName,
                CourierTypeId = shipmentStart.CourierTypeId,
                StartedAt = shipmentStart.ProcessDate
            }));
        }
        public async Task ShipmentFinish(ShipmentFinishDto shipmentFinish)
        {
            //third-party-calls or some business
            await _domainService.PublishEvent(new CustomEventData(Id, shipmentFinish.CorrelationId.ToString(), shipmentFinish.TransactionId.ToString(), new ShipmentFinished
            {
                FinishedAt = shipmentFinish.ProcessDate
            }));
            await _domainService.PublishEvent(new CustomEventData(Id, shipmentFinish.CorrelationId.ToString(), shipmentFinish.TransactionId.ToString(), new OrderCompleted
            {
                CompletedAt = shipmentFinish.ProcessDate
            }));
        }
    }
}