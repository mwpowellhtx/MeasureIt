namespace MeasureIt
{
    /// <summary>
    /// Publishing options.
    /// </summary>
    public interface IPublishingOptions
    {
        /// <summary>
        /// Gets or sets whether to PublishCounters.
        /// </summary>
        bool PublishCounters { get; set; }

        /// <summary>
        /// Gets or sets whether to ThrowPublishErrors.
        /// </summary>
        bool ThrowPublishErrors { get; set; }

        /// <summary>
        /// Gets or sets whether to PublishEvent.
        /// </summary>
        bool PublishEvent { get; set; }

        /// <summary>
        /// Gets whether MayProceed with Measurement Unabated.
        /// </summary>
        bool MayProceedUnabated { get; }
    }
}
