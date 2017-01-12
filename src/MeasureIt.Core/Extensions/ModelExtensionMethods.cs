using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public static class ModelExtensionMethods
    {
        /// <summary>
        /// Tries to Verify the <paramref name="type"/>, providing a
        /// <paramref name="failureMessage"/> upon failure.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="failureMessage"></param>
        /// <returns></returns>
        public delegate bool TryVerifyTypeDelegate(Type type, out string failureMessage);

        private static bool DefaultTryVerifyType(Type type, out string failureMessage)
        {
            failureMessage = null;
            return true;
        }

        /// <summary>
        /// Verifies that the <paramref name="type"/> is assignable to the
        /// <typeparamref name="TAssignableTo"/> type. <typeparamref name="TAssignableTo"/> may be
        /// anything, but is usually an interface.
        /// </summary>
        /// <typeparam name="TAssignableTo"></typeparam>
        /// <param name="type"></param>
        /// <param name="tryVerify"></param>
        /// <returns></returns>
        internal static Type VerifyType<TAssignableTo>(this Type type, TryVerifyTypeDelegate tryVerify = null)
        {
            tryVerify = tryVerify ?? DefaultTryVerifyType;

            var assignableToType = typeof(TAssignableTo);

            string message;

            if (!assignableToType.IsAssignableFrom(type))
            {
                message = string.Format("The type {0} must be assignable to {1}."
                    , type.FullName, assignableToType.FullName);
                throw new ArgumentException(message, "type");
            }

            if (!tryVerify(type, out message))
            {
                throw new ArgumentException(message);
            }

            return type;
        }

        /// <summary>
        /// Verifies that the <paramref name="types"/> are assignable to the
        /// <typeparamref name="TAssignableTo"/> type. <typeparamref name="TAssignableTo"/> may be
        /// anything, but is usually an interface.
        /// </summary>
        /// <typeparam name="TAssignableTo"></typeparam>
        /// <param name="types"></param>
        /// <returns></returns>
        internal static IEnumerable<Type> VerifyTypes<TAssignableTo>(this IEnumerable<Type> types)
        {
            return types.Select(t => VerifyType<TAssignableTo>(t));
        }

        internal static void IfNotNull(this IEnumerable<IPerformanceCounterAdapter> adapters
            , Action<IPerformanceCounterAdapter> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            adapters = adapters ?? new IPerformanceCounterAdapter[0];

            foreach (var adapter in adapters) action(adapter);
        }

        internal static T SetIf<T>(this T value, out T field, T defaultValue = default(T),
            Func<T, bool> predicate = null)
        {
            predicate = predicate ?? delegate { return true; };
            field = predicate(value) ? value : defaultValue;
            return field;
        }

        internal static string BuildMethodSignature<TReturn, TRoot>(this string methodName
            , Accessibility accessibility = Accessibility.None
            , Virtuality virtuality = Virtuality.None
            , PerformanceCounterType? counterType = null, string counterTypeName = null
            , params ParameterDescriptor[] args)
        {
            return methodName.BuildMethodSignature<TRoot>(typeof(TReturn), accessibility, virtuality, counterType, counterTypeName, args);
        }

        private static string BuildParameterSignature(this ParameterDescriptor arg)
        {
            return string.Format(@"{0} {1}", arg.ParameterType, arg.Name);
        }

        private static string Append(this string s, string b)
        {
            return string.Join(@" ", s, b).Trim();
        }

        internal static string BuildMethodSignature<TRoot>(this string methodName, Type returnType
            , Accessibility accessibility = Accessibility.None
            , Virtuality virtuality = Virtuality.None
            , PerformanceCounterType? counterType = null, string counterTypeName = null, params ParameterDescriptor[] args)
        {
            var decoration = !counterType.HasValue
                ? null
                : $"{counterType.Value.GetCounterTypeDecoration(counterTypeName)}({(counterType.Value.IsBaseCounterType() ? "Base" : string.Empty)})";

            var signature =
                $@"{returnType.FullName} {typeof(TRoot).FullName}.{methodName} ({string.Join(", ",
                    args.Select(a => a.BuildParameterSignature()))})".Trim();

            signature = accessibility.GetStringRepresentation()
                .Append(virtuality.GetStringRepresentation()).Append(signature);

            return string.IsNullOrEmpty(decoration)
                ? signature
                : $@"[{decoration}] {signature}";
        }

        [Obsolete]
        internal static string PrefixName<T>(this string root, Func<Type, string> prefixer = null)
        {
            return root.PrefixName(typeof(T), prefixer);
        }

        [Obsolete]
        internal static string PrefixName(this string root, Type type, Func<Type, string> prefixer = null)
        {
            prefixer = prefixer ?? (t => t.FullName);
            return string.Join(".", new[] {prefixer(type), root}.Where(s => !string.IsNullOrEmpty(s)));
        }

        private static readonly IDictionary<PerformanceCounterType, string> CounterTypeSuffixes;

        internal static string GetCounterTypeDecoration(this PerformanceCounterType value
            , string counterTypeName = null)
        {
            var suffixes = CounterTypeSuffixes;
            return counterTypeName ?? (suffixes.ContainsKey(value) ? suffixes[value] : null);
        }

        internal static bool IsBaseCounterType(this PerformanceCounterType value)
        {
            var bases = new[]
            {
                PerformanceCounterType.SampleBase
                , PerformanceCounterType.AverageBase
                , PerformanceCounterType.RawBase
                , PerformanceCounterType.CounterMultiBase
            };

            return bases.Contains(value);
        }

        // TODO: TBD: may not be necessary any longer...
        [Obsolete]
        internal static bool TryGetBaseSuffix(this PerformanceCounterType value, out string suffix)
        {
            const string @base = "Base";

            suffix = null;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (value)
            {
                case PerformanceCounterType.SampleBase:
                case PerformanceCounterType.AverageBase:
                case PerformanceCounterType.RawBase:
                case PerformanceCounterType.CounterMultiBase:
                    suffix = @base;
                    break;
            }

            return !string.IsNullOrEmpty(suffix);
        }

        static ModelExtensionMethods()
        {
            // TODO: TBD: or perhaps there is a better way to inject naming conventions/strategies...
            {
                const string numberofItemsHex = "NumberOfItemsHex";
                const string numberOfItems = "NumberOfItems";
                const string delta = "Delta";
                const string sample = "Sample";
                const string countPerTimeInterval = "CountPerTimeInterval";
                const string rateOfCountsPerSecond = "RateOfCountsPerSecond";
                const string rawFraction = "RawFraction";
                const string timer = "Timer";
                const string timerHundredNanoseconds = "Timer100Ns";
                const string sampleFraction = "SampleFraction";
                const string inverseTimer = "InverseTimer";
                const string inverseTimerHundredNanoseconds = "InverseTimer100Ns";
                const string multiTimer = "MultiTimer";
                const string multiTimerHundredNanoseconds = "MultiTimer100Ns";
                const string inverseMultiTimer = "InverseMultiTimer";
                const string inverseMultiTimerHundredNanoseconds = "InverseMultiTimer100Ns";
                const string averageTime = "AverageTime";
                const string elapsedTime = "ElapsedTime";
                const string average = "Average";
                //const string sampleBase = "Sample.Base";
                //const string averageBase = "Average.Base";
                //const string rawBase = "Raw.Base";
                //const string multiBase = "Multi.Base";

                var suffixes = new Dictionary<PerformanceCounterType, string>
                {
                    // number of items in hex (32-bit)
                    {PerformanceCounterType.NumberOfItemsHEX32, numberofItemsHex},
                    // number of items in hex (64-bit)
                    {PerformanceCounterType.NumberOfItemsHEX64, numberofItemsHex},
                    // number of items (32-bit)
                    {PerformanceCounterType.NumberOfItems32, numberOfItems},
                    // number of items (64-bit)
                    {PerformanceCounterType.NumberOfItems64, numberOfItems},
                    // delta (32-bit)
                    {PerformanceCounterType.CounterDelta32, delta},
                    // delta (64-bit)
                    {PerformanceCounterType.CounterDelta64, delta},
                    // sample
                    {PerformanceCounterType.SampleCounter, sample},
                    // count per time interval (32-bit)
                    {PerformanceCounterType.CountPerTimeInterval32, countPerTimeInterval},
                    // count per time interval (64-bit)
                    {PerformanceCounterType.CountPerTimeInterval64, countPerTimeInterval},
                    // rate of counts per second (32-bit)
                    {PerformanceCounterType.RateOfCountsPerSecond32, rateOfCountsPerSecond},
                    // rate of counts per second (64-bit)
                    {PerformanceCounterType.RateOfCountsPerSecond64, rateOfCountsPerSecond},
                    // raw fraction
                    {PerformanceCounterType.RawFraction, rawFraction},
                    // timer
                    {PerformanceCounterType.CounterTimer, timer},
                    // timer (100 nanoseconds)
                    {PerformanceCounterType.Timer100Ns, timerHundredNanoseconds},
                    // sample fraction
                    {PerformanceCounterType.SampleFraction, sampleFraction},
                    // inverse counter timer
                    {PerformanceCounterType.CounterTimerInverse, inverseTimer},
                    // inverse timer (100 nanoseconds)
                    {PerformanceCounterType.Timer100NsInverse, inverseTimerHundredNanoseconds},
                    // multi-timer
                    {PerformanceCounterType.CounterMultiTimer, multiTimer},
                    // multi-timer (100 nanoseconds)
                    {PerformanceCounterType.CounterMultiTimer100Ns, multiTimerHundredNanoseconds},
                    // inverse multi-timer
                    {PerformanceCounterType.CounterMultiTimerInverse, inverseMultiTimer},
                    // inverse multi-timer (100 nanoseconds)
                    {PerformanceCounterType.CounterMultiTimer100NsInverse, inverseMultiTimerHundredNanoseconds},
                    // average timer (32-bit)
                    {PerformanceCounterType.AverageTimer32, averageTime},
                    // elapsed time
                    {PerformanceCounterType.ElapsedTime, elapsedTime},
                    // average (64-bit)
                    {PerformanceCounterType.AverageCount64, average},
                    //// sample base
                    //{PerformanceCounterType.SampleBase, sampleBase},
                    //// average base
                    //{PerformanceCounterType.AverageBase, averageBase},
                    //// raw base
                    //{PerformanceCounterType.RawBase, rawBase},
                    //// counter multi-base
                    //{PerformanceCounterType.CounterMultiBase, multiBase}
                };

                CounterTypeSuffixes = new ConcurrentDictionary<PerformanceCounterType, string>(suffixes);
            }
        }
    }
}
