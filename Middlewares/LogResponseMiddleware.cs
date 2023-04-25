using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webAPIAutores.Middlewares
{
    public static class LogResponseMiddlewareExtension
    {
        public static IApplicationBuilder UseLogResponseHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogResponseHTTPMiddleware>();
        }
    }
        
    public class LogResponseHTTPMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LogResponseHTTPMiddleware> logger;

        public LogResponseHTTPMiddleware(RequestDelegate siguiente, ILogger<LogResponseHTTPMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

        //invoke || invokeAsync necesario para usar clase como middleware es usar uno de estos metodos

        public async Task InvokeAsync(HttpContext contexto)
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRepuesta = contexto.Response.Body;
                contexto.Response.Body = ms;

                await siguiente(contexto);
                //salida del mddleware
                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();

                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRepuesta);
                contexto.Response.Body = cuerpoOriginalRepuesta;

                logger.LogInformation(respuesta);
            }
        }

    }
}