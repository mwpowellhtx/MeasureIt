using System;

namespace MeasureIt.Castle.DynamicProxy
{
    /// <summary>
    /// Signals whether to Enable Interception for <see cref="Class"/> or <see cref="Interface"/>
    /// or both.
    /// </summary>
    [Flags]
    public enum AutofacEnableInterceptionOption : int
    {
        /// <summary>
        /// Signals to Enable Interception for Class.
        /// </summary>
        Class = 1 << 0,

        /// <summary>
        /// Signals to Enable Interception for Interface.
        /// </summary>
        Interface = 1 << 1
    }
}