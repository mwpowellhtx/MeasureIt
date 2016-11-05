using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace MeasureIt
{
    [PerformanceCounterCategory("Windsor", Help = "Windsor performance counter category"
        , CategoryType = PerformanceCounterCategoryType.MultiInstance)]
    public class WindsorPerformanceCounterCategoryAdapter : PerformanceCounterCategoryAdapterBase
    {
    }
}
