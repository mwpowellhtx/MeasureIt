namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISamplingOptions
    {
        /// <summary>
        /// Gets or sets whether ReadOnly.
        /// </summary>
        bool? ReadOnly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int? RandomSeed { get; }


        /// <summary>
        /// 
        /// </summary>
        double SampleRate { get; set; }
    }
}
