namespace MeasureIt
{
    public class SubjectClass
    {
        [PerformanceCounter("VirtualMethodDecorationOvershadowed"
            , typeof(DefaultPerformanceCounterCategoryAdapter)
            , typeof(AverageTimePerformanceCounterAdapter)
            , ReadOnly = true)]
        public virtual void VirtualMethodDecorationOvershadowed()
        {
        }

        [PerformanceCounter("VirtualMethodDecoratedInBaseOnly"
            , typeof(DefaultPerformanceCounterCategoryAdapter)
            , typeof(AverageTimePerformanceCounterAdapter)
            )]
        public virtual void VirtualMethodDecoratedInBaseOnly()
        {
        }

        public virtual void VirtualMethodDecoratedInDerivedClass()
        {
        }

        [PerformanceCounter("MethodDeclaredInBaseOnly"
            , typeof(DefaultPerformanceCounterCategoryAdapter)
            , typeof(AverageTimePerformanceCounterAdapter)
            )]
        public void MethodDeclaredInBaseOnly()
        {
        }
    }
}
