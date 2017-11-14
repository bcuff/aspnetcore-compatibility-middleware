using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreCompatibility;

namespace TestWebsite.Controllers
{
    [Produces("application/json")]
    [Route("test")]
    public class TestController : Controller
    {
        [Route("")]
        public async Task<object> Index()
        {
            var context = CompatibilityHttpContextAccessor.Current;
            context.Items["test"] = "foo";
            await Task.Yield();
            context = CompatibilityHttpContextAccessor.Current;
            if (context.Items["test"] != "foo") throw new Exception("failed");
            return new object();
        }
    }
}