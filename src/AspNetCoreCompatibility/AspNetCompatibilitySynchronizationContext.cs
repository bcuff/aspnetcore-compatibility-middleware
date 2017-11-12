using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreCompatibility
{
    public class AspNetCompatibilitySynchronizationContext : SynchronizationContext
    {
        readonly HttpContext _httpContext;
        readonly object _syncRoot = new object();
        readonly Queue<Action> _pendingOperations = new Queue<Action>();
        bool _executing;

        public AspNetCompatibilitySynchronizationContext(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public void ExecuteOrSchedule(Action action)
        {
            List<Exception> exceptions = null;
            lock (_syncRoot)
            {
                if (_executing)
                {
                    // defer the actions until the current operations complete
                    _pendingOperations.Enqueue(action);
                    return;
                }
                _executing = true;
            }
            // begin - only one thread at a time should run here
            using (new SynchronizationContextScope(this))
            using (HttpContextAccessor.OpenScope(_httpContext))
            {
                while (true)
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        if (exceptions == null) exceptions = new List<Exception>();
                        exceptions.Add(ex);
                    }
                    lock (_syncRoot)
                    {
                        if (_pendingOperations.Count > 0)
                        {
                            action = _pendingOperations.Dequeue();
                        }
                        else
                        {
                            _executing = false;
                            break;
                        }
                    }
                }
            }
            // end - only one thread at a time should run here
            if (exceptions != null) throw new AggregateException(exceptions);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            Action operation = () => d(state);
            bool scheduled = false;
            lock (_syncRoot)
            {
                if (_executing)
                {
                    _pendingOperations.Enqueue(operation);
                    scheduled = true;
                }
            }
            if (!scheduled)
            {
                Task.Run(() => ExecuteOrSchedule(operation));
            }
        }
    }
}
