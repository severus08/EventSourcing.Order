using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Order.Api.Filters
{
    public class CustomHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();
            if (context.ApiDescription.HttpMethod != "GET")
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "correlationId",
                    In = ParameterLocation.Header,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    },
                    Description = "",
                    Required = true
                });
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "transactionId",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                },
                Required = false
            });
        }
    }
}