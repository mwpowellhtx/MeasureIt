namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceCounterAdapterAttribute : IAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Help { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IPerformanceCounterAdapterDescriptor Descriptor { get; }
    }
}
