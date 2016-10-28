using System;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Descriptors
{
    using Fixtures;
    using Xunit;
    using BuildDescriptorAnonymousDelegate = Func<MethodInfo, bool>;

    public class MeasurePerformanceDescriptorTests : MeasurePerformanceDescriptorTestFixtureBase<
        MeasureMeasurePerformanceDescriptorFixture>
    {
        /// <summary>
        /// This is intentionally derived from <see cref="B"/>.
        /// </summary>
        private class A
        {
            [MeasurePerformance(typeof(CategoryFixture)
                , typeof(AverageTimePerformanceCounterAdapter)
                )]
            public virtual void Verify(bool a)
            {
            }

            // ReSharper disable once UnusedMember.Local, UnusedParameter.Local
            /// <summary>
            /// Another placeholder with an intentionally different signature.
            /// </summary>
            /// <param name="i"></param>
            [MeasurePerformance(typeof(CategoryFixture)
                , typeof(AverageTimePerformanceCounterAdapter)
                )]
            public void VerifyBase(int i)
            {
            }
        }

        /// <summary>
        /// This is intentionally derived from <see cref="A"/>.
        /// </summary>
        private class B : A
        {
            [MeasurePerformance(typeof(CategoryFixture)
                , typeof(AverageTimePerformanceCounterAdapter)
                )]
            public override void Verify(bool b)
            {
            }

            // ReSharper disable once UnusedMember.Local, UnusedParameter.Local
            /// <summary>
            /// Another placeholder with an intentionally different signature.
            /// </summary>
            /// <param name="s"></param>
            [MeasurePerformance(typeof(CategoryFixture)
                , typeof(AverageTimePerformanceCounterAdapter)
                )]
            public void VerifyDerived(string s)
            {
            }
        }

        /// <summary>
        /// This is intentionally unrelated to either <see cref="A"/> or <see cref="B"/>.
        /// </summary>
        private class C
        {
            // ReSharper disable once UnusedMember.Local, UnusedParameter.Local
            [MeasurePerformance(typeof(CategoryFixture)
                , typeof(AverageTimePerformanceCounterAdapter)
                )]
            public void Verify(bool c)
            {
            }
        }

        protected override MeasureMeasurePerformanceDescriptorFixture BuildDescriptor(Type type
            , BuildDescriptorAnonymousDelegate predicate)
        {
            Assert.NotNull(type);

            Assert.NotNull(predicate);

            var method = type.GetMethods().SingleOrDefault(predicate);

            Assert.NotNull(method);

            method.ReflectedType.Confirm(type);
            method.DeclaringType.Confirm(type);

            // This is a tiny snippet that would also contribute to the proper Discovery Agents.
            var descriptor = method
                .GetAttributeValues((MeasurePerformanceAttribute a) => a.Descriptor)
                .Select(d =>
                {
                    // We do not need to re-apply the Predicate.
                    d.Method = method;
                    // We will also need the RootType for test purposes.
                    d.RootType = type;
                    return d;
                }).SingleOrDefault();

            Assert.NotNull(descriptor);

            /* There is no Category Adapter Discover Agent involved in these unit tests, so we
             * do not expect there to be a type associated with the Descriptor itself. */

            descriptor.VerifyCounterCategoryAdapter();

            Assert.Collection(descriptor.AdapterDescriptors
                , a => Assert.Null(a.AdapterType)
                );

            return new MeasureMeasurePerformanceDescriptorFixture(descriptor);
        }

        private static class MethodNames
        {
            /// <summary>
            /// "Verify"
            /// </summary>
            internal const string Verify = "Verify";

            /// <summary>
            /// "VerifyBase"
            /// </summary>
            internal const string VerifyBase = "VerifyBase";

            /// <summary>
            /// "VerifyDerived"
            /// </summary>
            internal const string VerifyDerived = "VerifyDerived";
        }

        /// <summary>
        /// Verify that <see cref="MeasurePerformanceDescriptor"/> instances associated with
        /// related <paramref name="derivedType"/> and <paramref name="baseType"/> type methods
        /// <paramref name="derivedMethodName"/> and <paramref name="baseMethodName"/> are
        /// <see cref="IEquatable{PerformanceCounterDescriptor}.Equals(PerformanceCounterDescriptor)"/>.
        /// </summary>
        /// <param name="derivedType"></param>
        /// <param name="baseType"></param>
        /// <param name="derivedMethodName"></param>
        /// <param name="baseMethodName"></param>
        /// <param name="expectedEqual"></param>
        [Theory
        , InlineData(typeof(B), typeof(A), MethodNames.Verify, MethodNames.Verify, true)
        , InlineData(typeof(B), typeof(A), MethodNames.VerifyDerived, MethodNames.Verify, false)
        , InlineData(typeof(B), typeof(A), MethodNames.Verify, MethodNames.VerifyBase, false)
        , InlineData(typeof(B), typeof(A), MethodNames.VerifyDerived, MethodNames.VerifyBase, false)
        ]
        public void VerifyDescriptorEquality(Type derivedType, Type baseType
            , string derivedMethodName, string baseMethodName, bool expectedEqual)
        {
            derivedType.VerifySubclassOf(baseType);
            var baseDescriptor = BuildDescriptor(baseType, m => m.Name == baseMethodName);
            var derivedDescriptor = BuildDescriptor(derivedType, m => m.Name == derivedMethodName);
            // This is what we really want to verify here.
            Assert.Equal(baseDescriptor.IsSimilarTo(derivedDescriptor), expectedEqual);
            Assert.Equal(derivedDescriptor.IsSimilarTo(baseDescriptor), expectedEqual);
        }

        /// <summary>
        /// Verify that <see cref="MeasurePerformanceDescriptor"/> instances associated with
        /// unrelated <paramref name="firstType"/> and <paramref name="secondType"/> type methods
        /// <paramref name="firstMethodName"/> and <paramref name="secondMethodName"/> are not
        /// <see cref="IEquatable{PerformanceCounterDescriptor}.Equals(PerformanceCounterDescriptor)"/>.
        /// </summary>
        /// <param name="firstType"></param>
        /// <param name="secondType"></param>
        /// <param name="firstMethodName"></param>
        /// <param name="secondMethodName"></param>
        [Theory
        , InlineData(typeof(C), typeof(A), MethodNames.Verify, MethodNames.Verify)
        , InlineData(typeof(C), typeof(B), MethodNames.Verify, MethodNames.Verify)
        ]
        public void VerifyUnrelatedDescriptorInequality(Type firstType, Type secondType
            , string firstMethodName, string secondMethodName)
        {
            firstType.VerifyUnrelatedType(secondType);
            var firstDescriptor = BuildDescriptor(firstType, m => m.Name == firstMethodName);
            var secondDescriptor = BuildDescriptor(secondType, m => m.Name == secondMethodName);
            // This is what we really want to verify here.
            Assert.False(firstDescriptor.Equals(secondDescriptor));
            Assert.False(secondDescriptor.Equals(firstDescriptor));
        }
    }
}
