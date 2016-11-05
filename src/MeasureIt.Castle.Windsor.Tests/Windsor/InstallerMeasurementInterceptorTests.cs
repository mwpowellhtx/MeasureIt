using System;
using System.Reflection;

namespace MeasureIt.Castle.Windsor
{
    using Classes;
    using Interception;
    using Xunit;

    // TODO: TBD: besides verifying that callbacks happen, need to also verify that the category can be installed

    /// <summary>
    /// 
    /// </summary>
    // ReSharper disable UnusedParameter.Local
    public class InstallerMeasurementInterceptorTests : InstallerMeasurementInterceptorTestFixtureBase
    {
        private SubjectClass GetSubject()
        {
            var obj = new SubjectClass();

            var measured = Container.MeasureInstance<SubjectClass, IMeasurementInterceptor>(obj);

            Assert.NotNull(measured);

            // ReSharper disable once UseMethodIsInstanceOfType
            Assert.True(obj.GetType().IsAssignableFrom(measured.GetType()));

            // Having tested the Types, that is no mistake, since we are expecting a Proxy.
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
            // TODO: TBD: okay, so the plumbing is there to facilitate the measurement, but the category(ies) do indeed need to be installed...
            VerifyInvocation(GetSubject(), "Verify", obj => obj.Verify());
        }

        [Fact]
        public void SubjectCanBeValidated()
        {
            VerifyInvocation(GetSubject(), "Validate", obj => obj.Validate());
        }
    }
}
