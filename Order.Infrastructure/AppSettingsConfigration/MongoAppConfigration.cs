namespace Order.Infrastructure.AppSettingsConfigration
{
    public static class MongoAppConfigration
    {
        public static MongoAppConfigrationModel MongoConfig { get; set; } = new MongoAppConfigrationModel();
    }

    public class MongoAppConfigrationModel
    {
        public string ConnectionString { get; set; }
    }
}