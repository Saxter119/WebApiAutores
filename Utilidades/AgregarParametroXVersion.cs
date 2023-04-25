using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace webAPIAutores.Utilidades
{
    public class AgregarParametroXVersion : IOperationFilter
    {
        public void Apply( OpenApiOperation operation, OperationFilterContext context)
        {

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "cabezzzera",
                In = ParameterLocation.Header,
                Required = true
            });

        }
    }
}
