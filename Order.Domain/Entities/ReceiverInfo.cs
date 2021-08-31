using Order.Domain.ValueObjects;

namespace Order.Domain.Entities
{
    public class ReceiverInfo
    {
        public Customer Customer { get; set; }
        public bool DontRingBell { get; set; }
        public string AddressDetail { get; set; }
        
    }
}