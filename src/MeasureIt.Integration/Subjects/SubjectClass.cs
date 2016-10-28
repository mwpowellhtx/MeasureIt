namespace MeasureIt
{
    public class SubjectClass
    {
        [MeasurePerformance("VirtualMethodDecorationOvershadowed"
            , typeof(DefaultPerformanceCounterCategoryAdapter)
            , typeof(AverageTimePerformanceCounterAdapter)
            , ReadOnly = true)]
        public virtual void VirtualMethodDecorationOvershadowed()
        {
        }

        [MeasurePerformance("VirtualMethodDecoratedInBaseOnly"
            , typeof(DefaultPerformanceCounterCategoryAdapter)
            , typeof(AverageTimePerformanceCounterAdapter)
            )]
        public virtual void VirtualMethodDecoratedInBaseOnly()
        {
        }

        public virtual void VirtualMethodDecoratedInDerivedClass()
        {
        }

        [MeasurePerformance("MethodDeclaredInBaseOnly"
            , typeof(DefaultPerformanceCounterCategoryAdapter)
            , typeof(AverageTimePerformanceCounterAdapter)
            )]
        public void MethodDeclaredInBaseOnly()
        {
        }
    }
}
