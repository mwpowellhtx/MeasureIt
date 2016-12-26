namespace MeasureIt
{
    public class SubjectClassWithNonPublicMethods : SubjectClass
    {
        [MeasurePerformance("VirtualMethodDecorationOvershadowed"
            , typeof(DefaultPerformanceCounterCategoryAdapter)
            , typeof(AverageTimePerformanceCounterAdapter)
            , PublishCounters = false, PublishEvent = false
            , ThrowPublishErrors = true, ReadOnly = false, SampleRate = 0.25d)]
        public override void VirtualMethodDecorationOvershadowed()
        {
            base.VirtualMethodDecorationOvershadowed();
        }

        [MeasurePerformance("InternalTargetMethod"
            , typeof(DefaultPerformanceCounterCategoryAdapter)
            , typeof(AverageTimePerformanceCounterAdapter)
            , PublishCounters = false, ThrowPublishErrors = true
            , PublishEvent = false, SampleRate = 0d)]
        internal void InternalTargetMethod()
        {
        }

        public override void VirtualMethodDecoratedInBaseOnly()
        {
            base.VirtualMethodDecoratedInBaseOnly();
        }

        [MeasurePerformance("VirtualMethodDecoratedInDerivedClass"
            , typeof(DefaultPerformanceCounterCategoryAdapter)
            , typeof(AverageTimePerformanceCounterAdapter)
            )]
        public override void VirtualMethodDecoratedInDerivedClass()
        {
            base.VirtualMethodDecoratedInDerivedClass();
        }

        [MeasurePerformance("MethodDeclaredInDerivedOnly"
            , typeof(DefaultPerformanceCounterCategoryAdapter)
            , typeof(AverageTimePerformanceCounterAdapter)
            )]
        public void MethodDeclaredInDerivedOnly()
        {
        }
    }
}
