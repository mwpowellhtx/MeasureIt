namespace MeasureIt
{
    public class SubjectClass
    {
        [PerformanceCounter("VirtualMethodDecorationOvershadowed"
            , typeof(AverageTimePerformanceCounterAdapter)
            , typeof(DefaultPerformanceCounterCategoryAdapter), ReadOnly = true)]
        public virtual void VirtualMethodDecorationOvershadowed()
        {
        }

        [PerformanceCounter("VirtualMethodDecoratedInBaseOnly"
            , typeof(AverageTimePerformanceCounterAdapter)
            , typeof(DefaultPerformanceCounterCategoryAdapter))]
        public virtual void VirtualMethodDecoratedInBaseOnly()
        {
        }

        public virtual void VirtualMethodDecoratedInDerivedClass()
        {
        }

        [PerformanceCounter("MethodDeclaredInBaseOnly"
            , typeof(AverageTimePerformanceCounterAdapter)
            , typeof(DefaultPerformanceCounterCategoryAdapter))]
        public void MethodDeclaredInBaseOnly()
        {
        }
    }
}
