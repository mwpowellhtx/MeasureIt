using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace MeasureIt
{
    // TODO: TBD: I wonder, should this be names or better off being Types?
    /// <summary>
    /// PerformanceCounterTypes collections.
    /// </summary>
    [Obsolete] // TODO: TBD: much of this will be obsolete I think: but I do want to look up "default" PerformanceCounterType names as needs be
    internal class PerformanceCounterNames
    {
        private static Lazy<IDictionary<PerformanceCounterType, string>> _names;

        static PerformanceCounterNames()
        {
            _names = new Lazy<IDictionary<PerformanceCounterType, string>>(
                () => new ConcurrentDictionary<PerformanceCounterType, string>(
                    new Dictionary<PerformanceCounterType, string>
                    {
                        {PerformanceCounterType.AverageTimer32, "Average timer"},
                        {PerformanceCounterType.SampleBase, "(sample base)"},
                        {PerformanceCounterType.AverageBase, "(average base)"},
                        {PerformanceCounterType.RawBase, "(raw base)"},
                        {PerformanceCounterType.CounterMultiBase, "(counter multi-base)"},
                    }
                    )
                );
        }

        /// <summary>
        /// "AverageTimeTaken"
        /// </summary>
        public const string AverageTimeTaken = "AverageTimeTaken";

        /// <summary>
        /// "TotalNoOfOperations"
        /// </summary>
        public const string TotalNoOfOperations = "TotalNoOfOperations";

        /// <summary>
        /// "LastOperationExecutionTime"
        /// </summary>
        public const string LastOperationExecutionTime = "LastOperationExecutionTime";

        /// <summary>
        /// "NumberOfOperationsPerSecond"
        /// </summary>
        public const string NumberOfOperationsPerSecond = "NumberOfOperationsPerSecond";

        /// <summary>
        /// "NumberOfErrorsPerSecond"
        /// </summary>
        public const string NumberOfErrorsPerSecond = "NumberOfErrorsPerSecond";

        /// <summary>
        /// "CurrentConcurrentOperationsCount"
        /// </summary>
        public const string CurrentConcurrentOperationsCount = "CurrentConcurrentOperationsCount";

        /// <summary>
        /// Returns the StandardCounters.
        /// </summary>
        private static IEnumerable<string> GetStandardCounters()
        {
            yield return AverageTimeTaken;
            yield return TotalNoOfOperations;
            yield return LastOperationExecutionTime;
            yield return NumberOfOperationsPerSecond;
            yield return CurrentConcurrentOperationsCount;
        }

        /// <summary>
        /// LazyStandardCounters backing field.
        /// </summary>
        private static readonly Lazy<IEnumerable<string>> LazyStandardCounters
            = new Lazy<IEnumerable<string>>(GetStandardCounters);

        /// <summary>
        /// Gets the StandardCounters.
        /// </summary>
        public static IEnumerable<string> StandardCounters
        {
            get { return LazyStandardCounters.Value; }
        }
    }
}
