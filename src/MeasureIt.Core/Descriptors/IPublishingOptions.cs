namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPublishingOptions
    {
        /// <summary>
        /// 
        /// </summary>
        bool PublishCounters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool ThrowPublishErrors { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool PublishEvent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool MayProceedUnabated { get; }
    }
}
