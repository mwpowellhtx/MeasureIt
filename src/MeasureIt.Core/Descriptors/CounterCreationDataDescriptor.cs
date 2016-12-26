using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public class CounterCreationDataDescriptor : DescriptorBase, ICounterCreationDataDescriptor
    {
        /// <summary>
        /// Gets or sets the Adapter corresponding with the Descriptor.
        /// </summary>
        public IPerformanceCounterAdapter Adapter { get; set; }

        private string CalculateDescriptorName(IPerformanceCounterAdapter adapter,
            PerformanceCounterType counterType)
        {
            /* TODO: TBD: strategy for naming: first the counter prefix from the adapter, if possible
             * second, the signature of the decorated method if possible
             * third, the parent adapter guid id if possible;
             * fourth the counter descriptor guid id if necessary
             * include the counter type or whether base */

            string descriptorName;
            var counterDecoration = string.Empty;

            // TODO: TBD: not sure we should/would ever see this calculation apart from descriptors in their full context...
            if (adapter != null)
            {
                // Start from the Adapter Measurement first.
                descriptorName = adapter.Measurement != null ? adapter.Measurement.MemberSignature : Id.ToString("N");
                counterDecoration = $@"{adapter.Name}({(counterType.IsBaseCounterType() ? "Base" : string.Empty)})";
            }
            else
            {
                descriptorName = Id.ToString("N");
            }

            /* We always want to decorate a signature/name with the counter type. The only
             * question is whether it is the base counter type or the proper counter type. */

            return $@"[{counterDecoration}] {descriptorName}";
        }

        /// <summary>
        /// Gets the Descriptor Name.
        /// </summary>
        public string Name => CalculateDescriptorName(Adapter, CounterType);

        /// <summary>
        /// Gets or sets the Descriptor Help.
        /// </summary>
        public string Help { get; set; }

        /// <summary>
        /// Gets or sets the Descriptor CounterType.
        /// </summary>
        public PerformanceCounterType CounterType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CounterCreationDataDescriptor()
        {
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
            Help = other.Help;
            CounterType = other.CounterType;
            Adapter = other.Adapter;
        }

        /// <summary>
        /// Returns the creation data corresponding to the Descriptor.
        /// </summary>
        /// <returns></returns>
        public CounterCreationData GetCounterCreationData()
        {
            return new CounterCreationData(Name, Help, CounterType);
        }

        /// <summary>
        /// Returns a Clone of the object.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new CounterCreationDataDescriptor(this);
        }
    }
}
