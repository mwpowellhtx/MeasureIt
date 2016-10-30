using System;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DescriptorBase : IDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected DescriptorBase()
        {
            Id = Guid.NewGuid();
        }
    }
}
