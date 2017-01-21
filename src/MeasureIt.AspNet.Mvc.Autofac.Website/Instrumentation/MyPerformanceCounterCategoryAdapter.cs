namespace MeasureIt.AspNet.Mvc.Autofac.Instrumentation
{
    /// <summary>
    /// It is necessary to define this within the site assembly. Alternately, we could define it
    /// externally, but would need to update the .NET Framework references accordingly to align
    /// with the Mvc (and Autofac) framework versions.
    /// </summary>
    public class MyPerformanceCounterCategoryAdapter : PerformanceCounterCategoryAdapterBase
    {
    }
}
