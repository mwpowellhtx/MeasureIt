namespace MeasureIt.Castle.Interception.Measurement
{
    using Contexts;

    /// <summary>
    /// Measurement provider for Interception purposes.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IInterceptionMeasurementProvider<out TContext> : IMeasurementProvider<TContext>
        where TContext : class, IMeasurementContext
    {
    }

    /// <summary>
    /// Measurement provider for Interception purposes.
    /// </summary>
    public interface IInterceptionMeasurementProvider : IInterceptionMeasurementProvider<
        IInterceptionMeasurementContext>
    {
    }
}
