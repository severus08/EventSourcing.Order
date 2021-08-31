using System;

namespace Order.Infrastructure.AppSettingsConfigration
{
    public class ProjectSettings
    {
        public Guid ApplicationId { get; set; }
        public ModelCreationTypeEnum AggregateRootCreationType { get; set; }
        public ModelCreationTypeEnum ReadModelCreationType { get; set; }
    }

    public enum ModelCreationTypeEnum
    {
        Client = 1, // creation at workerservice
        Server = 2 // creation at server (eventStore resultMethod)
    }
}