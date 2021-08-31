namespace Order.Infrastructure.Attribute
{
    public class EventNameAttribute : System.Attribute
    {
        public string EventName { get; set; }

        public EventNameAttribute(string eventName)
        {
            this.EventName = eventName;
        }
    }
}