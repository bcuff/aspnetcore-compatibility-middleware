using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreCompatibility
{
    public sealed class HttpContextAccessor : IHttpContextAccessor
    {
        [ThreadStatic]
        static HttpContext _current;

        public static HttpContext Current
        {
            get => _current;
        }

        public HttpContext HttpContext
        {
            get => _current;
            set => throw new InvalidOperationException("Setting the context is only allowed by the middleware");
        }

        internal static IDisposable OpenScope(HttpContext httpContext)
        {
            if (_current != null) throw new InvalidOperationException("HttpContext.Current is already set up");
            _current = httpContext;
            return _scope;
        }

        static readonly Scope _scope = new Scope();

        private class Scope : IDisposable
        {
            public void Dispose()
            {
                _current = null;
            }
        }
    }
}
