namespace Order.Domain.Dto
{
    public class CancelOrderDto : OrderBaseDto
    {
        public int CancelReasonId { get; set; }
    }
}