using System;
using System.Reflection;

namespace MeasureIt.Descriptors
{
    using BuildDescriptorAnonymousDelegate = Func<MethodInfo, bool>;

    /// <summary>
    /// Provides some basic resources necessary for verifying
    /// <see cref="IPerformanceCounterDescriptor"/> based <see cref="IDescriptor"/> tests.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PerformanceCounterDescriptorTestFixtureBase<T> : DescriptorTestFixtureBase<T>
        where T : class
            , IEquatable<T>
            , IPerformanceCounterDescriptor
            , IEquatable<IPerformanceCounterDescriptor>
    {
        /// <summary>
        /// Returns the <see cref="T"/> <see cref="IPerformanceCounterDescriptor"/> based delegate
        /// corresponding with the <paramref name="type"/> and <paramref name="predicate"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected abstract T BuildDescriptor(Type type, BuildDescriptorAnonymousDelegate predicate);
    }
}
