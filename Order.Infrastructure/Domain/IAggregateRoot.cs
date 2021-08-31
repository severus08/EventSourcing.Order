using System;

namespace Order.Infrastructure.Domain
{
    public interface IAggregateRoot
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
    }
}