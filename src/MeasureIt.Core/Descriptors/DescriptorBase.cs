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

        /// <summary>
        /// Protected Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        protected DescriptorBase(DescriptorBase other)
        {
            Copy(other);
        }

        private void Copy(DescriptorBase other)
        {
            Id = other.Id;
        }

        /// <summary>
        /// Returns a Clone of this object.
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();
    }
}
