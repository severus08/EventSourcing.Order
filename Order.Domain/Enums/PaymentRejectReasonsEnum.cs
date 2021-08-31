using Order.Infrastructure.Enumaration;

namespace Order.Domain.Enums
{
    public class PaymentRejectReasonsEnum : Enumaration
    {
        public static readonly PaymentRejectReasonsEnum Fraud = new PaymentRejectReasonsEnum(1, "Fraud");
        public static readonly PaymentRejectReasonsEnum StolenCard = new PaymentRejectReasonsEnum(2, "StolenCard");
        public static readonly PaymentRejectReasonsEnum NoLimit = new PaymentRejectReasonsEnum(3, "NoLimit");

        public static readonly PaymentRejectReasonsEnum
            UnknownReason = new PaymentRejectReasonsEnum(4, "UnknownReason");

        private PaymentRejectReasonsEnum(int id, string name) : base(id, name)
        {
        }

        public static PaymentRejectReasonsEnum Create(int id)
        {
            return id switch
            {
                1 => PaymentRejectReasonsEnum.Fraud,
                2 => PaymentRejectReasonsEnum.StolenCard,
                3 => PaymentRejectReasonsEnum.NoLimit,
                _ => PaymentRejectReasonsEnum.UnknownReason
            };
        }
        public PaymentRejectReasonsEnum() { }
    }
}