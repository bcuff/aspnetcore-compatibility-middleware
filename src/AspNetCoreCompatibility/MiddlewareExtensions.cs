using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;

namespace AspNetCoreCompatibility
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Enables middleware that makes ASP .NET Core behave like the older ASP .NET.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The application builder.</returns>
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
