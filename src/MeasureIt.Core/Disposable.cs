using System;

namespace MeasureIt
{
    /// <summary>
    /// Disposable class definition.
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        /// <summary>
        /// Gets whether IsDisposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Disposed event.
        /// </summary>
        public event EventHandler<EventArgs> Disposed;

        /// <summary>
        /// Raises the <see cref="Disposed"/> event with the <paramref name="e"/> <see cref="EventArgs"/>.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseDisposed(EventArgs e)
        {
            if (Disposed == null) return;
            Disposed(this, e);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            RaiseDisposed(EventArgs.Empty);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            IsDisposed = true;
        }
    }
}
