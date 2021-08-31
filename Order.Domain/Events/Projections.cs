using System.Collections.Generic;
using Order.Domain.Repositories;
using Order.Infrastructure;
using Newtonsoft.Json;
using Order.Domain.Enums;

namespace Order.Domain.Events
{
    public static class Projections
    {
        private static readonly Dictionary<string, string> _registeredProjections = new Dictionary<string, string>()
        {
            {
                $"{OrderAggregateRootRepository.ORDER_PROJECTION_ORIGINAL_NAME}",
                OrderProjectionQueries.OrderProjection()
            }
        };

        public static readonly Dictionary<string, string> RegisteredProjections = Projections._registeredProjections;

        private static readonly Dictionary<string, string> _registeredPaymentNotifyProjections =
            new Dictionary<string, string>()
            {
                {
                    $"PaymentNotifyProjection-{OrderAggregateRootRepository.ORDER_PROJECTION_Version}",
                    NotifyProjectionQueries.PaymentNotifyProjection()
                }
            };

        public static readonly Dictionary<string, string> RegisteredPaymentNotifyProjections =
            Projections._registeredPaymentNotifyProjections;
        
        private static readonly Dictionary<string, string> _reporterProjections =
                    new Dictionary<string, string>()
                    {
                        {
                            $"TransactionCountReporterProjection-{OrderAggregateRootRepository.ORDER_PROJECTION_Version}",
                            ReportProjectionQueries.TransactionCountReporterProjection()
                        }
                    };
        
                public static readonly Dictionary<string, string> RegisteredReportedProjections =
                    Projections._reporterProjections;
        
    }

    public static class OrderProjectionQueries
    {
        #region OrderProjection

        /// <summary>
        /// state is aggregateRoot
        /// result is readModel
        /// </summary>
        private static string GetRawOrderQuery = @"
                |streamResultName|
                |source|
                |isPartition|
                .when({
                $init: function() {
                    return {
                        Id: '',                        
                        Status : null,            
                        StatusHistory : [],            
                        CancelReason : null,            
                        ReceiverInfo : {
                        Customer: { Id: null ,FullName:''},
                        DontRingBell : false,
                        AddressDetail : '',
                        },            
                        Products : [],
                        PaymentInfo : null,
                        CourierInfo : null,
                        IsPaid : false,
                        version : -1                           
                    }
                },
            'OrderCreated' : function(state, event) {
                    state.version += 1;
                    state.Id= event.streamId.replace('" + Constants.ORDER_STREAM_NAME + @"-','');
                    state.Products = event.body.DomainEvent.Products; 
                    state.ReceiverInfo.Customer.Id = event.body.DomainEvent.CustomerId;
                    state.ReceiverInfo.Customer.FullName = event.body.DomainEvent.CustomerFullName;                    
                    state.ReceiverInfo.DontRingBell = event.body.DomainEvent.DontRingBell;                    
                    state.ReceiverInfo.AddressDetail = event.body.DomainEvent.AddressDetail;
                    state.Status = {Id : 1 , Name : 'Order Initialized'};
                    state.StatusHistory.push({Id : 1 , Name : 'Order Initialized',ProcessDate : event.body.DomainEvent.CreatedAt});                    
                },
            'OrderCancelled' : function(state, event) {
                    state.version += 1;
                     
                    switch(event.body.DomainEvent.CancelReasonId) {
                      case 1:
                        state.CancelReason = {Id: 1 , name : 'OutofStock'};
                        break;                      
                      default:
                        state.CancelReason = {Id: 2 , name : 'UnknownReason'};
                    }
                    state.Status = {Id : 4 , Name : 'Order Cancelled'};
                    state.StatusHistory.push({Id : 4 , Name : 'Order Cancelled',ProcessDate : event.body.DomainEvent.CancelledAt});                    
                },
            'PaymentApproved' : function(state, event) {
                    state.version += 1;                    
                    state.PaymentInfo = {Id : event.body.DomainEvent.PaymentId , Reason : null};
                    state.IsPaid = true;
            },
            'PaymentRejected' : function(state, event) {
                    state.version += 1;                    
                    state.PaymentInfo = {Id : event.body.DomainEvent.PaymentId,};
                    state.IsPaid = false;
                    state.Status = {Id : 3 , Name : 'Order Cancelled'};
                    state.StatusHistory.push({Id : 3 , Name : 'Order Suspended',ProcessDate : event.body.DomainEvent.RejectedAt});
                    switch(event.body.DomainEvent.ReasonId) {
                      case 1:
                        state.PaymentInfo.Reason = {Id: 1 , name : 'Fraud'};
                        break;  
                      case 2:
                        state.PaymentInfo.Reason = {Id: 2 , name : 'StolenCard'};
                        break;  
                      case 3:
                        state.PaymentInfo.Reason = {Id: 3 , name : 'NoLimit'};
                        break; 
                      default:
                        state.PaymentInfo.Reason = {Id: 4 , name : 'UnknownReason'};
                        break;
                    }
            },            
            'ShipmentStarted' : function(state, event) {
                    state.version += 1;              

                    var courierType = '';
                    switch(event.body.DomainEvent.CourierTypeId) {
                      case 1:
                        courierType = {Id: 1 , name : 'CarCourier'};
                        break;  
                      case 2:
                        courierType = {Id: 2 , name : 'MotorCourier'};
                        break;  
                      default:
                        courierType = {Id: 1 , name : 'CarCourier'};
                        break;
                    }      

                    state.CourierInfo = {
                        Courier : { Id : event.body.DomainEvent.CourierId , CourierName: event.body.DomainEvent.CourierName } ,  
                        CourierType : courierType,
                        StartedAt : event.body.DomainEvent.StartedAt,
                        FinishedAt : null,
                        IsShipped : false
                    };                                     
                },
            'ShipmentFinished' : function(state, event) {
                    state.version += 1;                    
                    state.CourierInfo.FinishedAt = event.body.DomainEvent.FinishedAt;
                    state.CourierInfo.IsShipped = true;
                },
            'OrderCompleted' : function(state, event) {
                    state.version += 1;
                    state.Status = {Id : 3 , Name : 'Order Completed'};
                    state.StatusHistory.push({Id : 3 , Name : 'Order Completed',ProcessDate : event.body.DomainEvent.CompletedAt});                    
                },
            })
            .transformBy(function (state) {
                return {
                OrderId : state.Id ,
                OrderStatus : state.Status !== null ?
                {
                    Id : state.Status.Id,
                    Description : state.Status.Name
                } : null,
                StatusHistory : state.StatusHistory,
                CancelReason : state.CancelReason !== null
                    ?  
                    {
                        Id : state.CancelReason.Id,
                        Description : state.CancelReason.Name
                    }
                    : null,
                CustomerId : state.ReceiverInfo.Customer.Id,
                CustomerName : state.ReceiverInfo.Customer.FullName,
                DontRingBell : state.ReceiverInfo.DontRingBell,
                AddressDetail : state.ReceiverInfo.AddressDetail,
                Products : state.Products,
                PaymentTransactionId : state.PaymentInfo !== null ? state.PaymentInfo.Id : null,
                PaymentCancelReason : state.PaymentInfo !== null && state.PaymentInfo.Reason !== null
                    ?  
                    {
                        Id : state.PaymentInfo.Reason.Id,
                        Description : state.PaymentInfo.Reason.Name
                    }
                    : null,
                CourierId : state.CourierInfo !== null ? state.CourierInfo.Courier.Id : null,
                CourierName :state.CourierInfo !== null ? state.CourierInfo.Courier.CourierName : null,
                CourierTypeDescription : state.CourierInfo !== null ? state.CourierInfo.CourierType.Name : null,
                IsShipmentFinished : state.CourierInfo !== null ? state.CourierInfo.IsShipped : false,
                IsPaid : state.IsPaid
                };
            })
            .outputState()";

        public static string OrderProjection()
        {
            return GetRawOrderQuery.Replace("|source|", $"fromCategory('{Constants.ORDER_STREAM_NAME}')")
                .Replace("|isPartition|", ".foreachStream()")
                .Replace("|streamResultName|",
                    $"options({{ resultStreamName: '{OrderAggregateRootRepository.ORDER_PROJECTION_READ_MODEL_STREAM_NAME}' }})");
        }

        #endregion
    }

    public static class NotifyProjectionQueries
    {
        public static string PaymentNotifyStreamName = "PaymentNotify";

        private static string RawPaymentNotifyProjection => @"fromStreams(['$et-PaymentRejected','$et-PaymentApproved'])
            .when({
            $any: function(s, e) {
                linkTo('|streamName|-'+e.streamId.replace('|source|-',e.eventType+'-'), e)
            } 
        });";

        public static string PaymentNotifyProjection()
        {
            return RawPaymentNotifyProjection.Replace("|source|", Constants.ORDER_STREAM_NAME)
                .Replace("|streamName|", PaymentNotifyStreamName);
        }
    }

    public static class ReportProjectionQueries
    {
        public static string TransactionCountReporterStreamName = "totalcountreporterV2";

        private static string RawTransactionCountReporterProjection => @"
        |streamResultName|
        |source|
        .when({
          $init: function() {
            return {
              TotalOrderCount: 0,   
              CancelledOrderCount: 0,
              PaymentTransactionCount: 0,
              OnDeliveryOrderCount: 0,
              DeliveredOrderCount: 0
            }
          },
          OrderCreated : function(s, e) {
            s.TotalOrderCount += 1;
          },  
          PaymentApproved : function(s, e) {
            s.PaymentTransactionCount += 1;
          },
          PaymentRejected : function(s, e) {
            s.PaymentTransactionCount += 1;
          },  
          ShipmentStarted : function(s, e) {
            s.OnDeliveryOrderCount += 1;
          },
          ShipmentFinished : function(s, e) {
            s.OnDeliveryOrderCount -= 1;
            s.DeliveredOrderCount += 1;
          },  
          OrderCancelled : function(s, e) {
            s.CancelledOrderCount += 1;
          } 
        })
        .transformBy(function (s) {
            return {
              TotalOrderCount: s.TotalOrderCount,   
              CancelledOrderCount: s.CancelledOrderCount,
              PaymentTransactionCount: s.PaymentTransactionCount,
              OnDeliveryOrderCount: s.OnDeliveryOrderCount,
              DeliveredOrderCount: s.DeliveredOrderCount
            };
          });";

        public static string TransactionCountReporterProjection()
        {
            return RawTransactionCountReporterProjection
                .Replace("|source|", $"fromCategory('{Constants.ORDER_STREAM_NAME}')")
                .Replace("|streamResultName|", $"options({{ resultStreamName: '{TransactionCountReporterStreamName}' }})");
        }
    }
}