namespace MeasureIt.Castle.DynamicProxy
{
    using global::Castle.DynamicProxy;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>We also need to make a reference to <see cref="IInterceptor"/>.</remarks>
    public class AutofacProxyGenerationOptions : ProxyGenerationOptions
    {
        /// <summary>
        /// Gets or sets whether to Enable Interception for
        /// <see cref="AutofacEnableInterceptionOption.Class"/>,
        /// <see cref="AutofacEnableInterceptionOption.Interface"/>, or both.
        /// </summary>
        public AutofacEnableInterceptionOption EnableInterception { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AutofacProxyGenerationOptions()
        {
            EnableInterception = AutofacEnableInterceptionOption.Class;
        }
    }
}
