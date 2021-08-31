using AutoMapper;
using Order.Application.Commands;
using Order.Domain.Dto;

namespace Order.Application.Mapper
{

    public class CommandMapperProfile : Profile
    {
        public CommandMapperProfile()
        {
            CreateMap<CreateOrderCommand, CreateOrderDto>();
            CreateMap<CancelOrderCommand, CancelOrderDto>();
            CreateMap<PaymentApproveCommand, PaymentApproveDto>();
            CreateMap<PaymentRejectCommand, PaymentRejectDto>();
            CreateMap<ShipmentStartCommand, ShipmentStartDto>();
            CreateMap<ShipmentFinishCommand, ShipmentFinishDto>();
        }
    }
}