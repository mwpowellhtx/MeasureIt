using System;
using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PerformanceCounterCategoryAttribute : Attribute, IPerformanceCounterCategoryAttribute
    {
        private readonly Lazy<IPerformanceCounterCategoryDescriptor> _lazyDescriptor;

        public IPerformanceCounterCategoryDescriptor Descriptor
        {
            get { return _lazyDescriptor.Value; }
        }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name
        {
            get { return Descriptor.Name; }
            set { Descriptor.Name = value; }
        }

        /// <summary>
        /// Gets or sets the Help.
        /// </summary>
        public string Help
        {
            get { return Descriptor.Help; }
            set { Descriptor.Name = value; }
        }

        /// <summary>
        /// Gets or sets the CategoryType.
        /// </summary>
        public PerformanceCounterCategoryType CategoryType
        {
            get { return Descriptor.CategoryType; }
            set { Descriptor.CategoryType = value; }
        }

        private static string GetDefaultName()
        {
            return Guid.NewGuid().ToString("D");
        }

        /// <summary>
        /// 
        /// </summary>
        public PerformanceCounterCategoryAttribute()
            : this(GetDefaultName())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        public PerformanceCounterCategoryAttribute(string categoryName)
        {
            _lazyDescriptor = new Lazy<IPerformanceCounterCategoryDescriptor>(
                () => new PerformanceCounterCategoryDescriptor {Name = categoryName});
        }
    }
}
