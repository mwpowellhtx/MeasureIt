using System;

namespace MeasureIt
{
    public abstract class DisposableTestFixtureBase : IDisposable
    {
        protected bool IsDisposed { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
