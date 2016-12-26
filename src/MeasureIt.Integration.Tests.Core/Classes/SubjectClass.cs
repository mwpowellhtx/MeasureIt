namespace MeasureIt
{
    public class SubjectClass : SubjectClassBase
    {
        [MeasurePerformance(
            typeof(IntegrationPerformanceCounterCategoryAdapter)
            , typeof(TotalMemberAccessesPerformanceCounterAdapter)
            , ThrowPublishErrors = true
            )]
        public virtual void Validate()
        {
        }

        public virtual void DoesNotValidate()
        {
        }
    }
}
