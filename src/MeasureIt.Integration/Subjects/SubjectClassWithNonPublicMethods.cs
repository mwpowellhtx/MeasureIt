namespace MeasureIt
{
    public class SubjectClassWithNonPublicMethods : SubjectClass
    {
        [PerformanceCounter("VirtualMethodDecorationOvershadowed"
            , typeof(AverageTimePerformanceCounterAdapter)
            , typeof(DefaultPerformanceCounterCategoryAdapter)
            , PublishCounters = false, PublishEvent = false
            , ThrowPublishErrors = true, ReadOnly = false, SampleRate = 0.25d)]
        public override void VirtualMethodDecorationOvershadowed()
        {
            base.VirtualMethodDecorationOvershadowed();
        }

        [PerformanceCounter("InternalTargetMethod"
            , typeof(AverageTimePerformanceCounterAdapter)
            , typeof(DefaultPerformanceCounterCategoryAdapter)
            , PublishCounters = false, ThrowPublishErrors = true
            , PublishEvent = false, SampleRate = 0d)]
        internal void InternalTargetMethod()
        {
        }

        public override void VirtualMethodDecoratedInBaseOnly()
        {
            base.VirtualMethodDecoratedInBaseOnly();
        }

        [PerformanceCounter("VirtualMethodDecoratedInDerivedClass"
            , typeof(AverageTimePerformanceCounterAdapter)
            , typeof(DefaultPerformanceCounterCategoryAdapter))]
        public override void VirtualMethodDecoratedInDerivedClass()
        {
            base.VirtualMethodDecoratedInDerivedClass();
        }

        [PerformanceCounter("MethodDeclaredInDerivedOnly"
        , typeof(AverageTimePerformanceCounterAdapter)
        , typeof(DefaultPerformanceCounterCategoryAdapter))]
        public void MethodDeclaredInDerivedOnly()
        {
        }
    }
}
