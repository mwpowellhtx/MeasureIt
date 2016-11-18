using System;
using System.Reflection;
using Xunit;

namespace MeasureIt.Naming
{
    public abstract class NamingConventionTestFixtureBase<T> : TestFixtureBase
    {
        protected interface ITestOptions
        {
        }

        private static T DefaultFactory<TOptions>(TOptions options)
        {
            var type = typeof(T);

            const BindingFlags publicNonPublicInstance
                = BindingFlags.Public
                  | BindingFlags.NonPublic
                  | BindingFlags.Instance;

            var binder = Type.DefaultBinder;

            var ctor = type.GetConstructor(publicNonPublicInstance, binder, new Type[0], null);

            var obj = ctor.Invoke(new object[0]);

            Assert.NotNull(obj);
            Assert.Equal(obj.GetType(), type);

            return (T) obj;
        }

        protected virtual T CreateSubject<TOptions>(TOptions options, Func<TOptions, T> factory = null)
            where TOptions : ITestOptions
        {
            factory = factory ?? DefaultFactory;

            var instance = factory(options);

            Assert.NotNull(instance);

            return instance;
        }

        protected void DisposeIfNecessary(T obj)
        {
            Assert.NotNull(obj);

            var disposable = obj as IDisposable;

            if (disposable == null) return;

            disposable.Dispose();
        }
    }
}
