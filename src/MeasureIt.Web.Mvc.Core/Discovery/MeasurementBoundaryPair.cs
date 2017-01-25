using System;

namespace MeasureIt.Discovery
{
    using static MeasurementBoundary;

    /// <summary>
    /// Represents a Measurement Boundary pair.
    /// </summary>
    public class MeasurementBoundaryPair
    {
        /// <summary>
        /// <see cref="Begin"/> | <see cref="MeasurementBoundary.Action"/>
        /// </summary>
        private const MeasurementBoundary DefaultStart = Begin | MeasurementBoundary.Action;

        /// <summary>
        /// <see cref="End"/> | <see cref="MeasurementBoundary.Action"/>
        /// </summary>
        private const MeasurementBoundary DefaultStop = End | MeasurementBoundary.Action;

        /// <summary>
        /// Gets the Start Boundary of the Measurement. Defaults to <see cref="DefaultStart"/>.
        /// </summary>
        public MeasurementBoundary Start { get; private set; }

        /// <summary>
        /// Gets the Stop Boundary of the Measurement. Defaults to <see cref="DefaultStop"/>.
        /// </summary>
        public MeasurementBoundary Stop { get; private set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MeasurementBoundaryPair()
            : this(DefaultStart, DefaultStop)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        public MeasurementBoundaryPair(MeasurementBoundary start, MeasurementBoundary stop)
        {
            Set(start, stop);
        }

        /// <summary>
        /// Sets the <paramref name="start"/> and <paramref name="stop"/> values.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when there is any problem with either
        /// of the <paramref name="start"/> or <paramref name="stop"/> parameters.</exception>
        public MeasurementBoundaryPair Set(MeasurementBoundary start, MeasurementBoundary stop)
        {
            const MeasurementBoundary action = MeasurementBoundary.Action;

            /* This is sufficient. If we had any more than two, then possibly we consider a
             * more formal loop. But we don't. So just verify them and be done with it. */

            start.VerifyHasOne(Begin, End).VerifyHasOne(action, Result);
            stop.VerifyHasOne(Begin, End).VerifyHasOne(action, Result);

            // Then rule out any overlapping scenarios.
            if (start >= stop)
            {
                throw new ArgumentException(
                    $"'{typeof(MeasurementBoundary).FullName}' start value '{start}'"
                    + $" must occur prior to stop value '{stop}'.", nameof(start));
            }

            Start = start;
            Stop = stop;

            return this;
        }
    }
}
