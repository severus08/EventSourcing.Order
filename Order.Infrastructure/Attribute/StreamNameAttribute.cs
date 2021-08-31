namespace Order.Infrastructure.Attribute
{
    public class StreamNameAttribute : System.Attribute
    {
        public string StreamName { get; set; }

        public StreamNameAttribute(string streamName)
        {
            this.StreamName = streamName;
        }
    }
}