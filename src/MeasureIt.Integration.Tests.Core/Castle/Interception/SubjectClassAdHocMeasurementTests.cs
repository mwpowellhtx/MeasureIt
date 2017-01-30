namespace MeasureIt.Castle.Interception
{
    using Discovery;
    using Measurement;
    using Xunit;

    public class SubjectClassAdHocMeasurementTests : AdHocMeasurementTestFixtureBase<SubjectClass>
    {
        /// <summary>
        /// "Validate"
        /// </summary>
        private const string Validate = "Validate";

        // TODO: TBD: it might be sufficient to put these in the base class...

        private static void VerifyInvocation(InvocationInterceptedEventArgs e)
        {
            Assert.NotNull(e);
            Assert.NotNull(e.Invocation);
            Assert.Equal(Validate, e.Invocation.Method.Name);
        }

        protected override void SubjectClass_Intercepted(object sender, InvocationInterceptedEventArgs e)
        {
            VerifyInvocation(e);
            base.SubjectClass_Intercepted(sender, e);
        }

        protected override void SubjectClass_Measuring(object sender, InvocationInterceptedEventArgs e)
        {
            VerifyInvocation(e);
            base.SubjectClass_Measuring(sender, e);
        }

        protected override void SubjectClass_Measured(object sender, InvocationInterceptedEventArgs e)
        {
            VerifyInvocation(e);
            base.SubjectClass_Measured(sender, e);
        }

        protected override IInstrumentationDiscoveryOptions CreateDiscoveryOptions()
        {
            var options = base.CreateDiscoveryOptions();
            options.Assemblies = new[] {typeof(SubjectClass).Assembly};
            return options;
        }

        public SubjectClassAdHocMeasurementTests()
            : base(x => x.Validate())
        {
        }
    }
}
