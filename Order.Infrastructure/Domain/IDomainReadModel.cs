using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Order.Infrastructure.Domain
{
    public interface IDomainReadModel
    {
        public string Id { get; set; }
    }
}