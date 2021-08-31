namespace Order.Infrastructure.AppSettingsConfigration
{
    public static class EventStoreConfigration
    {
        public static EventStoreConfigrationModel EventStoreConfig { get; set; } = new EventStoreConfigrationModel();
    }

    public class EventStoreConfigrationModel
    {
        public string Uri { get; set; }
        public int Port { get; set; }
        public int ProjectionSetupPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}