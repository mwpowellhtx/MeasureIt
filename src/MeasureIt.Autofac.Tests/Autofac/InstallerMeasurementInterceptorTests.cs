using System;
using System.Reflection;
using MeasureIt.Castle;
using MeasureIt.Castle.Interception;
using MeasureIt.Castle.Interception.Measurement;

namespace MeasureIt
{
    using Autofac;
    using Discovery;
    using Xunit;

    /// <summary>
    /// 
    /// </summary>
    // ReSharper disable UnusedParameter.Local
    public class InstallerMeasurementInterceptorTests
        : InstallerMeasurementInterceptorTestFixtureBase<IContainer>
    {
        private ContainerBuilder Builder { get; set; }

        protected override void InitializeContainer()
        {
            var builder = Builder = new ContainerBuilder();

            Assert.NotNull(builder);

            // Autofact starts with a Container Builder, following which a Container is built.
            builder.EnableMeasurements<IInstallerInstrumentationDiscoveryService
                , InstallerInstrumentationDiscoveryService
                , MeasurementInterceptorFixture>(InitializeOptions);
        }

        protected override IContainer GetContainer()
        {
            Assert.NotNull(Builder);

            return Builder.Build();
        }

        protected override IInstallerInstrumentationDiscoveryService GetInterface()
        {
            var service = Container.Resolve<IInstallerInstrumentationDiscoveryService>();
            Assert.NotNull(service);
            return service;
        }

        protected override SubjectClass GetMeasuredSubject(SubjectClass obj)
        {
            /* It is the "same" code as for Autofac and yet it isn't so do not be fooled by the lines of code.
             * The similarity is by design. */

            var measured = Container.MeasureInstance<SubjectClass, IMeasurementInterceptor>(obj);
         
            Assert.NotNull(measured);

            return measured;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="method"></param>
        /// <param name="expectedMethodName"></param>
        private static void VerifyMethodInvoked<T>(T obj, MethodInfo method, string expectedMethodName)
        {
            Assert.NotNull(obj);
            Assert.NotNull(method);

            Assert.NotNull(expectedMethodName);
            Assert.NotEmpty(expectedMethodName);

            Assert.NotNull(method.DeclaringType);

            Assert.True(method.DeclaringType.IsAssignableFrom(typeof(T)));

            Assert.Equal(expectedMethodName, method.Name);
        }

        private static void VerifyInvocation<T>(T obj, string expectedMethodName, Action<T> action)
        {
            Assert.NotNull(obj);
            Assert.NotNull(action);

            MethodInfo intercepted = null;
            MethodInfo measuring = null;
            MethodInfo measured = null;

            using (new InvocationInterceptedContext(
                (sender, e) => intercepted = e.Invocation.Method
                , (sender, e) => measuring = e.Invocation.Method
                , (sender, e) => measured = e.Invocation.Method
                ))
            {
                Assert.Null(intercepted);

                action(obj);
            }

            VerifyMethodInvoked(obj, intercepted, expectedMethodName);
            VerifyMethodInvoked(obj, measuring, expectedMethodName);
            VerifyMethodInvoked(obj, measured, expectedMethodName);
        }

        [Fact]
        public void SubjectCanBeVerified()
        {
            // Verified this is working at time of writing. Pleasantly surprised by that.
            VerifyInvocation(GetSubject(), "Verify", obj => obj.Verify());
        }

        [Fact]
        public void SubjectCanBeValidated()
        {
            // Verified this is working at time of writing. Pleasantly surprised by that.
            VerifyInvocation(GetSubject(), "Validate", obj => obj.Validate());
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                Builder = null;
            }

            base.Dispose(disposing);
        }
    }
}
