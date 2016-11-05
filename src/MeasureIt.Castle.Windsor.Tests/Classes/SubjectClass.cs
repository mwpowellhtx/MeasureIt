namespace MeasureIt.Castle.Classes
{
    public class SubjectClass : SubjectClassBase
    {
        [MeasurePerformance(
            typeof(WindsorPerformanceCounterCategoryAdapter)
            , typeof(TotalMemberAccessesPerformanceCounterAdapter)
            )]
        public virtual void Validate()
        {
        }

        public virtual void DoesNotValidate()
        {
        }
    }
}
