namespace MeasureIt.Web.Mvc
{
    /// <summary>
    /// <see cref="StatefulStorage"/> mode.
    /// </summary>
    public enum StatefulStorageMode
    {
        /// <summary>
        /// PerApplication
        /// </summary>
        PerApplication,

        /// <summary>
        /// PerRequest
        /// </summary>
        PerRequest,

        /// <summary>
        /// PerSession
        /// </summary>
        PerSession
    }
}
