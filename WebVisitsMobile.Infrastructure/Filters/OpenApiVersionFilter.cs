using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace WebVisitsMobile.Infrastructure.Filters
{
    public class OpenApiVersionFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Forzar versión 3.0.3 usando reflexión (funciona en todas las versiones)
            var field = typeof(OpenApiDocument).GetField("_openApi", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(swaggerDoc, "3.0.3");
            }
            else
            {
                var prop = typeof(OpenApiDocument).GetProperty("OpenApi");
                if (prop != null && prop.CanWrite)
                    prop.SetValue(swaggerDoc, "3.0.3");
            }
        }
    }
}