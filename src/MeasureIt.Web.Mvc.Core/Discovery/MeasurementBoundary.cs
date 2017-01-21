using System;

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
            return (value & mask) != mask;
        }

        /// <summary>
        /// Returns whether <paramref name="value"/> DoesNotHave the <paramref name="mask"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        internal static bool TryDoesNotHave(this MeasurementBoundary value, MeasurementBoundary mask)
        {
            return (value & mask) == mask;
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
    }
}
