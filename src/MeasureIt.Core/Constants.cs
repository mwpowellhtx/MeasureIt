namespace MeasureIt
{
    internal static class Constants
    {
        /// <summary>
        /// 0d or 0 percent sample rate.
        /// </summary>
        internal const double MinSampleRate = 0d;

        /// <summary>
        /// 1d or 100 percent sample rate.
        /// </summary>
        internal const double MaxSampleRate = 1d;

        /// <summary>
        /// Based on <see cref="MaxSampleRate"/>.
        /// </summary>
        /// <see cref="MaxSampleRate"/>
        internal const double DefaultSampleRate = MaxSampleRate;
    }
}
