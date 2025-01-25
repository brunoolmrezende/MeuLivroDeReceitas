using Microsoft.OpenApi.Models;
using MyRecipeBook.API.Binders;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyRecipeBook.API.Filters
{
    public class IdsFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var encryptedIds = context
                .ApiDescription
                .ParameterDescriptions
                .Where(x => x.ModelMetadata.BinderType == typeof(MyRecipeBookBinder))
                .ToDictionary(d => d.Name, d => d);

            foreach (var paramater in operation.Parameters)
            {
                if (encryptedIds.TryGetValue(paramater.Name, out var apiParameter))
                {
                    paramater.Schema.Format = string.Empty;
                    paramater.Schema.Type = "string";
                }
            }

            foreach (var schema in context.SchemaRepository.Schemas.Values)
            {
                foreach (var property in schema.Properties)
                {
                    if (encryptedIds.TryGetValue(property.Key, out var apiParameter))
                    {
                        property.Value.Format = string.Empty;
                        property.Value.Type = "string";
                    }
                }
            }
        }
    }
}
