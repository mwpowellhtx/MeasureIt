using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MeasureIt
{
    using Contexts;
    using Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class PerformanceMeasurementDescriptor
        : DescriptorBase
            , IPerformanceMeasurementDescriptor
            , IEquatable<PerformanceMeasurementDescriptor>
    {
        private string _prefix;

        /// <summary>
        /// Gets or sets the Prefix.
        /// </summary>
        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        private static string CalculateMemberSignature(ref string prefix, Type rootType, MethodInfo method)
        {
            const string defaultPrefix = "";

            // So we do not want DeclaringType or even ReflectedType here, but rather rootType.
            return prefix = (!string.IsNullOrEmpty(prefix)
                ? prefix
                : (rootType == null || method == null
                    ? defaultPrefix
                    : method.GetMethodSignature(rootType, prefix)));
        }

        /// <summary>
        /// Gets or sets the MemberSignature.
        /// </summary>
        public string MemberSignature
        {
            get { return CalculateMemberSignature(ref _prefix, RootType, Method); }
            set { value.SetIf(out _prefix, string.Empty, s => !string.IsNullOrEmpty(s)); }
        }

        private Type _categoryType;

        private static bool TryVerifyCategoryType(Type type, out string message)
        {
            message = null;

            if (!type.IsClass || type.IsAbstract)
            {
                // Occurs when Not a Class or when Abstract (i.e. Not Concrete).
                message = string.Format(@"Type {0} must be a concrete class.", type);
            }

            return string.IsNullOrEmpty(message);
        }

        /// <summary>
        /// Gets or sets the CategoryType.
        /// </summary>
        public Type CategoryType
        {
            get { return _categoryType; }
            set
            {
                _categoryType = value.VerifyType<IPerformanceCounterCategoryAdapter>(TryVerifyCategoryType);

                //// TODO: TBD: leave "registering" or "unregistering" for the moment of discovery, discovery service(s), etc
                //UnregisterFromCategory(CategoryAdapter);
                //// TODO: TBD: the difficulty with a decoration/descriptor is that I'm not sure why we bother with a category adapter/attribute at all...
                //// TODO: I'm leaning towards removing it in simple favor of a "straight" hierarchy
                //var category = value.GetAttributeValue((PerformanceCounterCategoryAttribute a) => a.Descriptor);
                //RegisterWithCategory(CategoryAdapter = category);
            }
        }

        //private void RegisterWithCategory(IPerformanceCounterCategoryAdapter category)
        //{
        //    if (category == null) return;
        //    category.Register(this);
        //}

        //private void UnregisterFromCategory(IPerformanceCounterCategoryAdapter category)
        //{
        //    if (category == null) return;
        //    category.Unregister(this);
        //}

        /// <summary>
        /// Gets or sets the CategoryAdapter.
        /// </summary>
        public virtual IPerformanceCounterCategoryAdapter CategoryAdapter { get; set; }

        private IEnumerable<Type> _adapterTypes;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Type> AdapterTypes
        {
            get { return _adapterTypes; }
            private set
            {
                _adapterTypes = value.VerifyTypes<IPerformanceCounterAdapter>();

                foreach (var a in Adapters) a.Dispose();

                _adapters = CreateAdapters().ToList();
            }
        }

        private IList<IPerformanceCounterAdapter> _adapters;

        /// <summary>
        /// Gets the adapters.
        /// </summary>
        public IEnumerable<IPerformanceCounterAdapter> Adapters
        {
            get { return _adapters; }
            private set { _adapters = (value ?? new List<IPerformanceCounterAdapter>()).ToList(); }
        }

        private IList<IPerformanceCounterAdapter> PrivateAdapters
        {
            get
            {
                return _adapters.ToBidirectionalList(
                    added => added.Measurement = this
                    , removed => removed.Measurement = null
                    );
            }
        }

        /// <summary>
        /// Gets or sets whether ReadOnly.
        /// </summary>
        public bool? ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the InstanceLifetime.
        /// </summary>
        public PerformanceCounterInstanceLifetime InstanceLifetime { get; set; }

        /// <summary>
        /// Gets or sets whether to PublishCounters.
        /// </summary>
        public bool PublishCounters { get; set; }

        /// <summary>
        /// Gets or sets whether to ThrowPublishErrors.
        /// </summary>
        public bool ThrowPublishErrors { get; set; }

        /// <summary>
        /// Gets or sets whether to PublishEvent.
        /// </summary>
        public bool PublishEvent { get; set; }

        /// <summary>
        /// Gets whether MayProceedUnabated.
        /// </summary>
        /// <see cref="PublishCounters"/>
        /// <see cref="PublishEvent"/>
        public bool MayProceedUnabated => !(PublishCounters || PublishEvent);

        private double _sampleRate;

        /// <summary>
        /// Gets the SampleRate for the Descriptor.
        /// </summary>
        public double SampleRate
        {
            get { return _sampleRate; }
            set
            {
                const double min = Constants.MinSampleRate;
                const double max = Constants.MaxSampleRate;
                _sampleRate = Math.Max(min, Math.Min(max, value));
            }
        }

        /// <summary>
        /// Gets or sets the RootType.
        /// </summary>
        public Type RootType { get; set; }

        private MethodInfo _method;

        /// <summary>
        /// Gets or sets the Method.
        /// </summary>
        public MethodInfo Method
        {
            get { return _method; }
            set
            {
                _method = value;
                // TODO: TBD: may need a Prefix after all? or just use its successor alone...
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        public PerformanceMeasurementDescriptor(Type categoryType, Type adapterType, params Type[] otherAdapterTypes)
            : this(null, categoryType, adapterType, otherAdapterTypes)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        public PerformanceMeasurementDescriptor(string prefix, Type categoryType, Type adapterType,
            params Type[] otherAdapterTypes)
        {
            Initialize(prefix, categoryType, adapterType, otherAdapterTypes);
        }

        internal PerformanceMeasurementDescriptor(IPerformanceMeasurementDescriptor other)
        {
            Copy(other);
        }

        // TODO: TBD: do nothing with Prefix?
        private void Initialize(string prefix, Type categoryType, Type adapterType,
            params Type[] otherAdapterTypes)
        {
            _adapters = new List<IPerformanceCounterAdapter>();

            CategoryType = categoryType;
            AdapterTypes = new[] {adapterType}.Concat(otherAdapterTypes);

            InstanceLifetime = PerformanceCounterInstanceLifetime.Process;

            PublishCounters = true;
            PublishEvent = true;
            ThrowPublishErrors = false;

            SampleRate = Constants.DefaultSampleRate;
        }

        private void Copy(IPerformanceMeasurementDescriptor other)
        {
            _adapters = new List<IPerformanceCounterAdapter>();
            AdapterTypes = other.AdapterTypes.ToArray();

            CategoryType = other.CategoryType;
            CategoryAdapter = other.CategoryAdapter;

            InstanceLifetime = other.InstanceLifetime;

            RootType = other.RootType;
            Method = other.Method;

            PublishCounters = other.PublishCounters;
            PublishEvent = other.PublishEvent;
            ThrowPublishErrors = other.ThrowPublishErrors;

            SampleRate = other.SampleRate;

            ReadOnly = other.ReadOnly;
        }

        private IEnumerable<IPerformanceCounterAdapter> CreateAdapters()
        {
            // TODO: TBD: might do with relaying Options here...
            const BindingFlags publicNonPublicInstance
                = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var binder = Type.DefaultBinder;

            var ctors = AdapterTypes.Select(a => a.GetConstructor(publicNonPublicInstance,
                binder, new Type[0], null)).ToArray();

            if (!ctors.Any())
                throw new InvalidOperationException("Invalid descriptor instance.");

            var adapters = PrivateAdapters;

            foreach (IPerformanceCounterAdapter adapter in ctors.Select(ctor => ctor.Invoke(new object[0])))
            {
                adapters.Add(adapter);
                yield return adapter;
            }
        }

        /// <summary>
        /// Gets the Exception.
        /// </summary>
        public virtual Exception Exception { get; private set; }

        /// <summary>
        /// Gets whether HasError.
        /// </summary>
        public virtual bool HasError => Exception != null;

        /// <summary>
        /// Sets the Error to the <paramref name="ex"/>.
        /// </summary>
        /// <param name="ex"></param>
        public virtual void SetError(Exception ex)
        {
            Exception = ex;
        }

        /// <summary>
        /// Creates a Context corresponding with the Descriptor.
        /// </summary>
        /// <returns></returns>
        public virtual IPerformanceMeasurementContext CreateContext()
        {
            return new PerformanceMeasurementContext(this, Adapters.ToArray());
        }

        /// <summary>
        /// Returns whether <paramref name="x"/> Equals <paramref name="y"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected static bool Equals(IPerformanceMeasurementDescriptor x,
            IPerformanceMeasurementDescriptor y)
        {
            // Rule the instances in or out based on these criteria.
            if (x == null || y == null) return false;

            if (ReferenceEquals(x, y)) return true;

            {
                var comparer = new MethodInfoEqualityComparer();

                // Ensure that the Method Base Definitions are in agreement.
                if (!comparer.Equals(x.Method.GetBaseDefinition(),
                    y.Method.GetBaseDefinition())) return false;
            }

            /* Cannot be considered if the Descriptors are not even related by lineage.
             * This is different from Similarity which requires only requires lineage. */

            if (x.RootType == null || y.RootType == null
                || x.RootType != y.RootType) return false;

            // Must be aligned with the same CategoryType.
            if (x.CategoryType == null || y.CategoryType == null
                || x.CategoryType != y.CategoryType) return false;

            {
                /* Last but not least ensure that the AdapterTypes are aligned.
                 * In my estimation, does not matter the order of the types, per se. */

                var comparer = new TypeEqualityComparer();

                return x.AdapterTypes.Count() == y.AdapterTypes.Count()
                       && x.AdapterTypes.OrderBy(t => t.FullName)
                           .Zip(y.AdapterTypes.OrderBy(t => t.FullName), (a, b) => new {a, b})
                           .All(z => comparer.Equals(z.a, z.b));
            }
        }

        /// <summary>
        /// Returns whether this instance Equals the <paramref name="other"/> one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IPerformanceMeasurementDescriptor other)
        {
            return Equals(this, other);
        }

        /// <summary>
        /// Returns whether this instance Equals the <paramref name="other"/> one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PerformanceMeasurementDescriptor other)
        {
            return Equals(this, other);
        }

        /// <summary>
        /// Returns a Clone of the object.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new PerformanceMeasurementDescriptor(this);
        }
    }

    internal static class PerformanceMeasurementDescriptorExtensionMethods
    {
        /// <summary>
        /// Returns the Signature corresponding with the <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static string GetParameterSignature(this ParameterInfo parameter)
        {
            var attrib = string.Empty;
            var keyword = string.Empty;
            var defaultValue = string.Empty;

            if (parameter.ParameterType.IsByRef)
                keyword = parameter.IsOut ? "out" : "ref";
            else if (parameter.IsOut)
                attrib = "[Out]";
            else if (parameter.IsIn)
                attrib = "[In]";

            if (parameter.GetCustomAttributes<ParamArrayAttribute>().Any()) keyword = "params";

            // ReSharper disable once InvertIf
            if (parameter.IsOptional)
            {
                /* I believe this will get the default case done. Assume first of all that
                 * the Parameter may be Null. Then decide whether the Parameter is a String.
                 * Otherwise, just take the Default As-Is. */

                if (parameter.DefaultValue == null)
                    defaultValue = @"null";
                else if (parameter.DefaultValue is string)
                    defaultValue = $@"""{parameter.DefaultValue}""";
                else
                    defaultValue = $@"{parameter.DefaultValue}";

                if (!string.IsNullOrEmpty(defaultValue))
                    defaultValue = @" = " + defaultValue;
            }

            // Mind the spacing amid elements.
            return string.Join(@" ", attrib, keyword, parameter.ParameterType.FullName,
                parameter.Name, defaultValue).Trim();
        }

        private static string Append(this string s, string t)
        {
            return string.Join(@" ", s, t).Trim();
        }

        // TODO: TBD: include visibility...
        /// <summary>
        /// Returns the Signature corresponding with the <paramref name="method"/>.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="rootType"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        internal static string GetMethodSignature(this MethodInfo method, Type rootType, string prefix = null)
        {
            var accessibility = string.Empty;
            var virtuality = string.Empty;

            const string @public = "public";
            const string @private = "private";
            const string @protected = "protected";
            const string @internal = "internal";

            const string @sealed = "sealed";
            const string @override = "override";
            const string @virtual = "virtual";

            if (method.IsPrivate)
                accessibility = accessibility.Append(@private);
            // TODO: TBD: may need/want to leverage IsFamilyAnd/OrAssembly...
            if (method.IsFamily)
                accessibility = accessibility.Append(@protected);
            if (method.IsAssembly)
                accessibility = accessibility.Append(@internal);
            if (method.IsPublic)
                accessibility = accessibility.Append(@public);

            if (method.IsFinal)
                virtuality = virtuality.Append(@sealed);
            if (method.GetBaseDefinition().DeclaringType != method.DeclaringType)
                virtuality = virtuality.Append(@override);
            else if (method.IsVirtual)
                virtuality = virtuality.Append(@virtual);

            const string dot = @".";

            var fullName = string.IsNullOrEmpty(prefix)
                ? string.Join(dot, rootType.FullName, method.Name)
                : string.Join(dot, rootType.FullName, prefix, method.Name);

            var args = method.GetParameters().Select(a => a.GetParameterSignature());

            var sig = accessibility
                .Append(virtuality)
                .Append(string.Join(@" ", method.ReturnType.FullName, fullName,
                    string.Join(string.Join(", ", args), @"(", @")")));

            return sig;
        }
    }
}
