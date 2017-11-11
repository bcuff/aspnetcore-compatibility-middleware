using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AspNetCoreCompatibility
{
    /// <summary>
    /// For using an alternate SynchronizationContext on the current thread.
    /// </summary>
    internal class SynchronizationContextScope : IDisposable
    {
        readonly SynchronizationContext _old;

        /// <summary>
        /// Creates a new SynchronizationContextScope.
        /// When the scope is opened the current thread adopts the new context.
        /// When Dispose is called the context is restored to its previous value.
        /// </summary>
        /// <param name="context">The new context to use within the scope.</param>
        public SynchronizationContextScope(SynchronizationContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            _old = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(context);
        }

        /// <summary>
        /// Restores the old synchronization context if there was one.
        /// </summary>
        public void Dispose()
        {
            SynchronizationContext.SetSynchronizationContext(_old);
        }
    }
}
