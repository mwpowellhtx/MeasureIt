namespace MeasureIt.Castle.Interception.Measurement
{
    using Contexts;

    public interface IInterceptionMeasurementProvider<out TContext> : IMeasurementProvider<TContext>
        where TContext : class, IMeasurementContext
    {
    }

    public interface IInterceptionMeasurementProvider : IInterceptionMeasurementProvider<
        IInterceptionMeasurementContext>
    {
    }
}
