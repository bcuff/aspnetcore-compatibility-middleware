using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using AspNetCoreCompatibility;

namespace Tests
{
    [TestFixture]
    public class SynchronizationContextTests
    {
        [Test]
        public Task HttpContext_should_live_across_await()
        {
            var target = new MockHttpContext();
            Task t = null;

            async Task Execute()
            {
                Assert.AreSame(target, HttpContextAccessor.Current);
                await Task.Yield();
                Assert.AreSame(target, HttpContextAccessor.Current);
                await Task.Delay(1);
                Assert.AreSame(target, HttpContextAccessor.Current);
            }

            new AspNetCompatibilitySynchronizationContext(target)
                .ExecuteOrSchedule(() =>
                {
                    t = Execute();
                });

            return t;
        }
    }
}
