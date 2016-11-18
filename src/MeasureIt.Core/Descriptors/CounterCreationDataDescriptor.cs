using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public class CounterCreationDataDescriptor : DescriptorBase, ICounterCreationDataDescriptor
    {
        public IPerformanceCounterAdapter Adapter { get; set; }

        private string _suffix;

        public string Suffix
        {
            get { return _suffix; }
            set
            {
                const string defaultSuffix = null;
                _suffix = string.IsNullOrEmpty(value) ? defaultSuffix : value;
            }
        }

        private static IEnumerable<char> Separators
        {
            get
            {
                /* Much of the time Separators will be Period, but there may be others, such as
                 * forward or backslash. */

                yield return '.';
                yield return '/';
                yield return '\\';
                yield return ',';
                yield return ';';
                yield return ':';
                yield return '?';
                yield return '&';
                yield return '#';
                yield return '%';
                yield return '*';
                yield return '|';
            }
        }

        /// <summary>
        /// Returns the aligned Parts contributed from
        /// <see cref="IPerformanceCounterAdapter.Name"/> and
        /// <see cref="IPerformanceMeasurementDescriptor.Prefix"/> if possible, and
        /// <see cref="Suffix"/>.
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="counterType"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetNameParts(IPerformanceCounterAdapter adapter,
            PerformanceCounterType counterType, string suffix)
        {
            const StringSplitOptions opts = StringSplitOptions.RemoveEmptyEntries;

            var seps = Separators.ToArray();

            if (adapter != null)
            {
                if (adapter.Measurement != null)
                {
                    foreach (var part in adapter.Measurement.Prefix.Split(seps, opts))
                        yield return part;
                }
            }

            if (!string.IsNullOrEmpty(suffix))
            {
                foreach (var part in suffix.Split(seps, opts))
                    yield return part;
            }

            var shouldIncludeCounterTypeInName
                = !(adapter == null || adapter.Measurement == null)
                  && adapter.Measurement.ShouldIncludeCounterTypeInName;

            if (shouldIncludeCounterTypeInName)
            {
                // In which case "Base" should be included in the name, when necessary.
                foreach (var part in counterType.GetCounterTypeSuffixes().Split(seps, opts))
                {
                    if (string.IsNullOrEmpty(part)) continue;
                    yield return part;
                }
            }
            else
            {
                string baseSuffix;
                if (counterType.TryGetBaseSuffix(out baseSuffix))
                    yield return baseSuffix;
            }
        }

        public string Name
        {
            get { return string.Join(".", GetNameParts(Adapter, CounterType, Suffix)); }
        }

        public string Help { get; set; }

        public PerformanceCounterType CounterType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CounterCreationDataDescriptor()
            : this((string) null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="suffix"></param>
        public CounterCreationDataDescriptor(string suffix)
        {
            Suffix = suffix;
            Help = string.Empty;
        }

        private CounterCreationDataDescriptor(CounterCreationDataDescriptor other)
            : base(other)
        {
            Copy(other);
        }

        private void Copy(CounterCreationDataDescriptor other)
        {
            // Moniker not the Name, per se.
            Suffix = other.Suffix;
            Help = other.Help;
            CounterType = other.CounterType;
            Adapter = other.Adapter;
        }

        public CounterCreationData GetCounterCreationData()
        {
            return new CounterCreationData(Name, Help, CounterType);
        }

        public override object Clone()
        {
            return new CounterCreationDataDescriptor(this);
        }
    }
}
