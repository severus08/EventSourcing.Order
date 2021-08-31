using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Order.Api.Requests
{
    public abstract class BaseRequest
    {
        [JsonIgnore]
        public Guid TransactionId { get; set; }
        [JsonIgnore]
        public Guid CorrelationId { get; set; }

        public void FillMetaData(IHeaderDictionary headers)
        {
            CorrelationId = Guid.Parse(headers["correlationId"].ToString());
            TransactionId = headers.ContainsKey("transactionId") ?
                Guid.Parse(headers["transactionId"].ToString()) : Guid.NewGuid();
        }
    }
}