using Microsoft.AspNetCore.Http;
using System.Text;

namespace WebVisitsMobile.Infrastructure.Middlewares
{
    public class SwaggerVersionMiddleware
    {
        private readonly RequestDelegate _next;

        public SwaggerVersionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Capturar el flujo de salida original
            var originalBodyStream = context.Response.Body;
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context);

            // Si la ruta es de swagger.json y es exitosa
            if (context.Request.Path.Value?.Contains("/swagger/") == true &&
                context.Request.Path.Value?.EndsWith("swagger.json") == true &&
                context.Response.StatusCode == 200)
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                // Reemplazar la versión inválida por una válida
                var correctedBody = responseBody.Replace("\"openapi\": \"3.0.4\"", "\"openapi\": \"3.0.3\"");

                // Escribir la respuesta corregida
                context.Response.Body = originalBodyStream;
                context.Response.ContentLength = Encoding.UTF8.GetByteCount(correctedBody);
                await context.Response.WriteAsync(correctedBody);
            }
            else
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }
    }
}