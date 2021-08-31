namespace Order.Api.Requests
{
    public class CancelOrderRequest : BaseRequest
    {
        public int CancelReasonId { get; set; }
    }
}