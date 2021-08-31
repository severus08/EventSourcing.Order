using System;
using Bogus;
using Order.Api.Requests;

namespace Order.Api.IntegrationTests.ObjectMothers
{
    public class RequestMothers
    {
        public static readonly string Locale = "tr"; //CultureInfo.CurrentCulture.Name;

        public static class CreateOrderRequestMothers
        {
            public static CreateOrderRequest Default(Guid? correlationId = null, Guid? transactionId = null) =>
                new Faker<CreateOrderRequest>(Locale)
                    .RuleFor(order => order.OrderId, bogus => bogus.Random.Guid())
                    .RuleFor(order => order.CustomerId, bogus => bogus.Random.Guid())
                    .RuleFor(order => order.CustomerFullName, bogus => bogus.Person.FullName)
                    .RuleFor(order => order.Seller, bogus => bogus.Company.CompanyName())
                    .RuleFor(order => order.Products, bogus =>
                        new Faker<ProductDto>()
                            .RuleFor(product => product.ProductId, bogus.Random.Guid)
                            .RuleFor(product => product.Quantity, bogus.Random.Int(1, 5))
                            .RuleFor(product => product.UnitPrice, bogus.Random.Int(10, 30))
                            .Generate(bogus.Random.Int(1, 5))
                    )
                    .RuleFor(order => order.DontRingBell, bogus => bogus.Random.Bool())
                    .RuleFor(order => order.AddressDetail, bogus => bogus.Address.SecondaryAddress())
                    .RuleFor(order => order.TransactionId, bogus => transactionId ?? bogus.Random.Guid())
                    .RuleFor(order => order.CorrelationId, bogus => correlationId ?? bogus.Random.Guid());
            public static CreateOrderRequest GenerateWithCustomerProductCount(int productCount,Guid? correlationId = null, Guid? transactionId = null) =>
                new Faker<CreateOrderRequest>(Locale)
                    .RuleFor(order => order.OrderId, bogus => bogus.Random.Guid())
                    .RuleFor(order => order.CustomerId, bogus => bogus.Random.Guid())
                    .RuleFor(order => order.CustomerFullName, bogus => bogus.Person.FullName)
                    .RuleFor(order => order.Seller, bogus => bogus.Company.CompanyName())
                    .RuleFor(order => order.Products, bogus =>
                        new Faker<ProductDto>()
                            .RuleFor(product => product.ProductId, bogus.Random.Guid)
                            .RuleFor(product => product.Quantity, bogus.Random.Int(1, 5))
                            .RuleFor(product => product.UnitPrice, bogus.Random.Int(10, 30))
                            .Generate(productCount)
                    )
                    .RuleFor(order => order.DontRingBell, bogus => bogus.Random.Bool())
                    .RuleFor(order => order.AddressDetail, bogus => bogus.Address.SecondaryAddress())
                    .RuleFor(order => order.TransactionId, bogus => transactionId ?? bogus.Random.Guid())
                    .RuleFor(order => order.CorrelationId, bogus => correlationId ?? bogus.Random.Guid());
        }

        public static class CancelOrderRequestMothers
        {
            public static CancelOrderRequest Default(Guid? correlationId = null, Guid? transactionId = null) =>
                new Faker<CancelOrderRequest>(Locale)
                    .RuleFor(order => order.CancelReasonId, bogus => bogus.Random.Int(1, 2))
                    .RuleFor(order => order.TransactionId, bogus => transactionId ?? bogus.Random.Guid())
                    .RuleFor(order => order.CorrelationId, bogus => correlationId ?? bogus.Random.Guid());

            public static CancelOrderRequest GenerateWithCustomReason(int reasonId, Guid? correlationId = null,
                Guid? transactionId = null) =>
                new Faker<CancelOrderRequest>(Locale)
                    .RuleFor(order => order.CancelReasonId, bogus => reasonId).RuleFor(order => order.TransactionId,
                        bogus => transactionId ?? bogus.Random.Guid())
                    .RuleFor(order => order.CorrelationId, bogus => correlationId ?? bogus.Random.Guid());
        }

        public static class PaymentApproveRequestMothers
        {
            public static PaymentApproveRequest Default(Guid? correlationId = null, Guid? transactionId = null) =>
                new Faker<PaymentApproveRequest>(Locale)
                    .RuleFor(order => order.PaymentId, bogus => bogus.Random.Guid())
                    .RuleFor(order => order.Amount, bogus => bogus.Random.Int(1, 100))
                    .RuleFor(order => order.TransactionId, bogus => transactionId ?? bogus.Random.Guid())
                    .RuleFor(order => order.CorrelationId, bogus => correlationId ?? bogus.Random.Guid());

            public static PaymentApproveRequest GenerateWithCustomAmount(decimal amount, Guid? correlationId = null,
                Guid? transactionId = null) =>
                new Faker<PaymentApproveRequest>(Locale)
                    .RuleFor(order => order.PaymentId, bogus => bogus.Random.Guid())
                    .RuleFor(order => order.Amount, bogus => amount)
                    .RuleFor(order => order.TransactionId, bogus => transactionId ?? bogus.Random.Guid())
                    .RuleFor(order => order.CorrelationId, bogus => correlationId ?? bogus.Random.Guid());
        }

        public static class PaymentRejectRequestMothers
        {
            public static PaymentRejectRequest Default(Guid? correlationId = null, Guid? transactionId = null) =>
                new Faker<PaymentRejectRequest>(Locale)
                    .RuleFor(order => order.PaymentId, bogus => bogus.Random.Guid())
                    .RuleFor(order => order.ReasonId, bogus => bogus.Random.Int(1, 3))
                    .RuleFor(order => order.TransactionId, bogus => transactionId ?? bogus.Random.Guid())
                    .RuleFor(order => order.CorrelationId, bogus => correlationId ?? bogus.Random.Guid());

            public static PaymentRejectRequest GenerateWithReasonId(int reasonId, Guid? correlationId = null,
                Guid? transactionId = null) =>
                new Faker<PaymentRejectRequest>(Locale)
                    .RuleFor(order => order.PaymentId, bogus => bogus.Random.Guid())
                    .RuleFor(order => order.ReasonId, bogus => reasonId)
                    .RuleFor(order => order.TransactionId, bogus => transactionId ?? bogus.Random.Guid())
                    .RuleFor(order => order.CorrelationId, bogus => correlationId ?? bogus.Random.Guid());
        }

        public static class ShipmentStartRequestMothers
        {
            public static ShipmentStartRequest Default(Guid? correlationId = null, Guid? transactionId = null) =>
                new Faker<ShipmentStartRequest>(Locale)
                    .RuleFor(order => order.CourierId, bogus => bogus.Random.Guid())
                    .RuleFor(order => order.CourierName, bogus => bogus.Person.FullName)
                    .RuleFor(order => order.CourierTypeId, bogus => bogus.Random.Int(1, 2))
                    .RuleFor(order => order.TransactionId, bogus => transactionId ?? bogus.Random.Guid())
                    .RuleFor(order => order.CorrelationId, bogus => correlationId ?? bogus.Random.Guid());

            public static ShipmentStartRequest GenerateWithCourierType(int courierType, Guid? correlationId = null,
                Guid? transactionId = null) =>
                new Faker<ShipmentStartRequest>(Locale)
                    .RuleFor(order => order.CourierId, bogus => bogus.Random.Guid())
                    .RuleFor(order => order.CourierName, bogus => bogus.Person.FullName)
                    .RuleFor(order => order.CourierTypeId, bogus => courierType)
                    .RuleFor(order => order.TransactionId, bogus => transactionId ?? bogus.Random.Guid())
                    .RuleFor(order => order.CorrelationId, bogus => correlationId ?? bogus.Random.Guid());
        }

        public static class ShipmentFinishRequestMothers
        {
            public static ShipmentFinishRequest Default(Guid? correlationId = null, Guid? transactionId = null) =>
                new Faker<ShipmentFinishRequest>(Locale)
                    .RuleFor(order => order.TransactionId, bogus => transactionId ?? bogus.Random.Guid())
                    .RuleFor(order => order.CorrelationId, bogus => correlationId ?? bogus.Random.Guid());
        }
    }
}