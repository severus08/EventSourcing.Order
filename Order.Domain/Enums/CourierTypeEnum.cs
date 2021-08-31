using Order.Infrastructure.Enumaration;

namespace Order.Domain.Enums
{
    public class CourierTypeEnum: Enumaration
    {
        public static readonly CourierTypeEnum CarCourier = new CourierTypeEnum(1, "CarCourier");
        public static readonly CourierTypeEnum MotorCourier = new CourierTypeEnum(2, "MotorCourier");
        private CourierTypeEnum(int id, string name) : base(id, name)
        {
        }
        public static CourierTypeEnum Create(int id)
        {
            return id switch
            {
                1 => CourierTypeEnum.CarCourier,
                2 => CourierTypeEnum.MotorCourier,
                _ => CourierTypeEnum.CarCourier
            };
        }
        public CourierTypeEnum() { }
    }
}