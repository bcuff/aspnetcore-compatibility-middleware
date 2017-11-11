using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;

namespace AspNetCoreCompatibility
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseAspNetCompatibility(this IApplicationBuilder app)
            => app.Use((context, next) =>
            {
                var c = new AspNetCompatibilitySynchronizationContext(context);
                Task t = null;
                c.ExecuteOrSchedule(() => { t = next(); });
                return t;
            });
    }
}
