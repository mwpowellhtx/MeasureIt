namespace MeasureIt.Castle.DynamicProxy
{
    using global::Castle.DynamicProxy;
    /// <summary>
    /// <see cref="ProxyGenerationOptions"/> for use with Autofac.
    /// </summary>
    public interface IAutofacProxyGenerationOptions
    {
        /// <summary>
        /// Gets or sets whether to Enable Interception for
        /// <see cref="AutofacEnableInterceptionOption.Class"/>,
        /// <see cref="AutofacEnableInterceptionOption.Interface"/>, or both.
        /// </summary>
        AutofacEnableInterceptionOption EnableInterception { get; set; }
    }
}
