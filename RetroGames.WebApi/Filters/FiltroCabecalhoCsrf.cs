using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RetroGames.WebApi.Filters;

public sealed class FiltroCabecalhoCsrf : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.HttpMethod is not ("POST" or "PUT" or "DELETE"))
        {
            return;
        }

        operation.Parameters ??= [];
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-CSRF-TOKEN",
            In = ParameterLocation.Header,
            Required = true,
            Description = "Use o requestToken retornado por GET /api/Autenticacao/csrf.",
            Schema = new OpenApiSchema { Type = JsonSchemaType.String },
        });
    }
}
