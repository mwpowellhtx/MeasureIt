using System;
using System.Reflection;

namespace MeasureIt.Descriptors
{
    using BuildDescriptorAnonymousDelegate = Func<MethodInfo, bool>;

    /// <summary>
    /// Provides some basic resources necessary for verifying
    /// <see cref="IMeasurePerformanceDescriptor"/> based <see cref="IDescriptor"/> tests.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MeasurePerformanceDescriptorTestFixtureBase<T> : DescriptorTestFixtureBase<T>
        where T : class
            , IEquatable<T>
            , IMeasurePerformanceDescriptor
            , IEquatable<IMeasurePerformanceDescriptor>
    {
        /// <summary>
        /// Returns the <see cref="T"/> <see cref="IMeasurePerformanceDescriptor"/> based delegate
        /// corresponding with the <paramref name="type"/> and <paramref name="predicate"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected abstract T BuildDescriptor(Type type, BuildDescriptorAnonymousDelegate predicate);
    }
}
