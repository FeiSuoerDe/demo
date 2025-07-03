using System;
using System.Collections.Generic;
using demo.addons.GDTask.Internal;

namespace demo.addons.GDTask
{
    /// <summary>
    /// Lightweight IProgress[T] factory.
    /// </summary>
    public static class Progress
    {
        public static IProgress<T> Create<T>(Action<T>? handler)
        {
            return handler == null ? NullProgress<T>.Instance : new AnonymousProgress<T>(handler);
        }

        public static IProgress<T> CreateOnlyValueChanged<T>(Action<T>? handler, IEqualityComparer<T>? comparer = null)
        {
            return handler == null ? NullProgress<T>.Instance : new OnlyValueChangedProgress<T?>(handler, comparer ?? GodotEqualityComparer.GetDefault<T>());
        }

        private sealed class NullProgress<T> : IProgress<T>
        {
            public static readonly IProgress<T> Instance = new NullProgress<T>();

            private NullProgress()
            {

            }

            public void Report(T value)
            {
            }
        }

        private sealed class AnonymousProgress<T>(Action<T>? action) : IProgress<T>
        {
            public void Report(T value)
            {
                action?.Invoke(value);
            }
        }

        private sealed class OnlyValueChangedProgress<T>(Action<T?>? action, IEqualityComparer<T?> comparer)
            : IProgress<T>
        {
            private bool _isFirstCall = true;
            private T? _latestValue;

            public void Report(T? value)
            {
                if (_isFirstCall)
                {
                    _isFirstCall = false;
                }
                else if (comparer.Equals(value, _latestValue))
                {
                    return;
                }

                _latestValue = value;
                action?.Invoke(value);
            }
        }
    }
}