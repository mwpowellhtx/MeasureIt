using System;

namespace MeasureIt.Contexts
{
    /// <summary>
    /// TwoStageMeasurementContext interface.
    /// </summary>
    public interface ITwoStageMeasurementContext : IMeasurementContext
    {
        /// <summary>
        /// Starts a new Measurement.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the Measurement Context from running.
        /// </summary>
        /// <returns></returns>
        ITwoStageMeasurementContext Stop();

        /// <summary>
        /// Sets the <see cref="Exception"/> for the Measurement Context.
        /// </summary>
        /// <param name="ex"></param>
        void SetError(Exception ex);
    }
}
