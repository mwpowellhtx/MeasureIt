namespace MeasureIt.Castle.Classes
{
    public class SubjectClass : SubjectClassBase
    {
        [MeasurePerformance(
            typeof(WindsorPerformanceCounterCategoryAdapter)
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
