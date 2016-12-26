namespace MeasureIt.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMeasurementContext : IContext
    {
        // TODO: TBD: Descriptor? or simply "options" ?
        /// <summary>
        /// Gets the Descriptor.
        /// </summary>
        IPerformanceMeasurementDescriptor Descriptor { get; }
    }

    internal static class MeasurementContextExtensionMethods
    {
        /// <summary>
        /// Returns whether the calling scope MayReturn depending on the nature of the
        /// <paramref name="context"/>. Returning from the scope may not be appropriate in all use
        /// cases, so hence the recommended, not mandatory, verbiage.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static bool MayReturn(this IMeasurementContext context)
        {
            return context == null || context.Descriptor.MayProceedUnabated;
        }
    }
}
