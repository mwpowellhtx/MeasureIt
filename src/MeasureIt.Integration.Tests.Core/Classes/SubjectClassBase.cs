using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// The same rules apply utilizing Dynamic Proxy here as with any other proxy usage. Members
    /// must be public and virtual, or protected internal, in order to be interception eligible.
    /// </summary>
    public abstract class SubjectClassBase
    {
        [MeasurePerformance(
            typeof(IntegrationPerformanceCounterCategoryAdapter)
            , typeof(TotalMemberAccessesPerformanceCounterAdapter)
            , InstanceLifetime = PerformanceCounterInstanceLifetime.Process
            , ThrowPublishErrors = true
            )]
        public virtual void Verify()
        {
        }

        public virtual void DoesNotVerify()
        {
        }
    }
}
