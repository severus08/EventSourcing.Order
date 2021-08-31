using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Order.Infrastructure.Attribute;
using Order.Infrastructure.Domain;

namespace Order.Infrastructure.Extensions
{
    public static class IAggregateRootExtensions
    {
        public static string GetStreamName(this IAggregateRoot aggregateRoot)
        {
            return ((StreamNameAttribute)StreamNameAttribute.GetCustomAttribute(aggregateRoot.GetType(), typeof(StreamNameAttribute))).StreamName;
        }
    }
}