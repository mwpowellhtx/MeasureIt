using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Discovery
{
    using static MvcInstrumentationDiscoveryOptions;

    /// <summary>
    /// Represents the Measurement Boundary. These are specified in pairs.
    /// </summary>
    [Flags]
    public enum MeasurementBoundary
    {
        /// <summary>
        /// Begin (0x1)
        /// </summary>
        Begin = Bases.Base << Shifts.Begin,

        /// <summary>
        /// End (0x2)
        /// </summary>
        End = Bases.Base << Shifts.End,

        /// <summary>
        /// Action (0x4)
        /// </summary>
        Action = Bases.Base << Shifts.Action,

        /// <summary>
        /// Result (0x8)
        /// </summary>
        Result = Bases.Base << Shifts.Result,

        /// <summary>
        /// BeginAction (0x5)
        /// </summary>
        BeginAction = Begin | Action,

        /// <summary>
        /// BeginAction (0x6)
        /// </summary>
        EndAction = End | Action,

        /// <summary>
        /// BeginResult (0x9)
        /// </summary>
        BeginResult = Begin | Result,

        /// <summary>
        /// BeginResult (0xa)
        /// </summary>
        EndResult = End | Result,
    }

    internal static class MeasurementBoundaryExtensionMethods
    {
        /// <summary>
        /// Returns whether <paramref name="value"/> Has the <paramref name="mask"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        internal static bool TryHas(this MeasurementBoundary value, MeasurementBoundary mask)
        {
            // Logic was backwards but is now correct.
            return (value & mask) == mask;
        }

        /// <summary>
        /// Returns whether <paramref name="value"/> DoesNotHave the <paramref name="mask"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        internal static bool TryDoesNotHave(this MeasurementBoundary value, MeasurementBoundary mask)
        {
            // Logic was backwards but is now correct.
            return (value & mask) != mask;
        }

        /// <summary>
        /// Verifies whether <paramref name="value"/> Has the <paramref name="mask"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/>
        /// <see cref="TryDoesNotHave"/> the <paramref name="mask"/>.</exception>
        internal static MeasurementBoundary VerifyHaving(this MeasurementBoundary value, MeasurementBoundary mask)
        {
            if (value.TryDoesNotHave(mask))
            {
                throw new ArgumentException($"'{value}' should have '{mask}'.", nameof(value));
            }

            return value;
        }

        /// <summary>
        /// Verifies whether <paramref name="value"/> DoesNotHave the <paramref name="mask"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/>
        /// <see cref="TryHas"/> the <paramref name="mask"/>.</exception>
        internal static MeasurementBoundary VerifyNotHaving(this MeasurementBoundary value, MeasurementBoundary mask)
        {
            if (value.TryHas(mask))
            {
                throw new ArgumentException($"'{value}' should not have '{mask}'.", nameof(value));
            }

            return value;
        }

        /// <summary>
        /// Verifies that <paramref name="value"/> has one of the <paramref name="masks"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="masks"></param>
        /// <returns></returns>
        /// <remarks>This is a sufficient verification. No need to go checking having or not
        /// having all.</remarks>
        internal static MeasurementBoundary VerifyHasOne(this MeasurementBoundary value,
            params MeasurementBoundary[] masks)
        {
            if (masks.Any() && masks.Where(x => value.TryHas(x)).Count() == 1) return value;

            var maskStrs = string.Join(", ", masks.Select(x => $"'{x}'"));

            throw new ArgumentException(
                $"'{typeof(MeasurementBoundary).FullName}' value '{value}'"
                + $" must contain exactly one of [{maskStrs}].", nameof(value));
        }

        /// <summary>
        /// Verifies that the <paramref name="values"/> has exactly two items and returns the
        /// first as <paramref name="start"/> and the second as <paramref name="stop"/>.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        internal static void VerifyBoundaries(this IEnumerable<MeasurementBoundary> values,
            out MeasurementBoundary start, out MeasurementBoundary stop)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values), "values is null.");
            }

            // ReSharper disable once PossibleMultipleEnumeration
            if (values.Count() != 2)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                var valuesStr = string.Join(", ", values.Select(x => $"'{x}'"));

                // ReSharper disable once PossibleMultipleEnumeration
                throw new ArgumentException(
                    $"Values [{valuesStr}] ({values.Count()})"
                    + " must contain exactly two items.", nameof(values));
            }

            // Just extract the first and second values. No need to verify anything yet.

            // ReSharper disable once PossibleMultipleEnumeration
            start = values.ElementAt(0);
            // ReSharper disable once PossibleMultipleEnumeration
            stop = values.ElementAt(1);
        }
    }
}
