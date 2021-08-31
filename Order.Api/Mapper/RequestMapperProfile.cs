using AutoMapper;
using Order.Api.Requests;
using Order.Application.Commands;
using Order.Domain.Entities;

namespace Order.Api.Mapper
{
    public class RequestMapperProfile : Profile
    {
        public RequestMapperProfile()
        {
            CreateMap<CreateOrderRequest, CreateOrderCommand>();
            CreateMap<CancelOrderRequest, CancelOrderCommand>();
            CreateMap<PaymentApproveRequest, PaymentApproveCommand>();
            CreateMap<PaymentRejectRequest, PaymentRejectCommand>();
            CreateMap<ShipmentStartRequest, ShipmentStartCommand>();
            CreateMap<ShipmentFinishRequest, ShipmentFinishCommand>();
            CreateMap<ProductDto, Product>();
        }
    }
}