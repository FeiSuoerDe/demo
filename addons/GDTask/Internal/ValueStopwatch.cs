using System;
using System.Diagnostics;

namespace demo.addons.GDTask.Internal
{
    internal readonly struct ValueStopwatch
    {
        private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

        private readonly long _startTimestamp;

        public static ValueStopwatch StartNew() => new ValueStopwatch(Stopwatch.GetTimestamp());

        private ValueStopwatch(long startTimestamp)
        {
            this._startTimestamp = startTimestamp;
        }

        public TimeSpan Elapsed => TimeSpan.FromTicks(this.ElapsedTicks);

        public bool IsInvalid => _startTimestamp == 0;

        public long ElapsedTicks
        {
            get
            {
                if (_startTimestamp == 0)
                {
                    throw new InvalidOperationException("Detected invalid initialization(use 'default'), only to create from StartNew().");
                }

                var delta = Stopwatch.GetTimestamp() - _startTimestamp;
                return (long)(delta * TimestampToTicks);
            }
        }
    }
}