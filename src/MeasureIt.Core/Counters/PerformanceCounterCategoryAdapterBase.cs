using System;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PerformanceCounterCategoryAdapterBase : IPerformanceCounterCategoryAdapter
    {
        private readonly Lazy<IPerformanceCounterCategoryDescriptor> _lazyDescriptor;

        public IPerformanceCounterCategoryDescriptor Descriptor
        {
            get { return _lazyDescriptor.Value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected PerformanceCounterCategoryAdapterBase()
        {
            _lazyDescriptor = new Lazy<IPerformanceCounterCategoryDescriptor>(
                () => this.GetAttributeValue((PerformanceCounterCategoryAttribute a) => a.Descriptor));
        }
    }
}
