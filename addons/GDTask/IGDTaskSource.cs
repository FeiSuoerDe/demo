using System;
using System.Runtime.CompilerServices;

namespace demo.addons.GDTask
{
    public enum GdTaskStatus
    {
        /// <summary>The operation has not yet completed.</summary>
        Pending = 0,
        /// <summary>The operation completed successfully.</summary>
        Succeeded = 1,
        /// <summary>The operation completed with an error.</summary>
        Faulted = 2,
        /// <summary>The operation completed due to cancellation.</summary>
        Canceled = 3
    }

    // General architecture:
    // Each GDTask holds a IGDTaskSource, which determines how the GDTask will run. This is basically a strategy pattern.
    // GDTask is a struct, so will be allocated on stack with no garbage collection. All IGDTaskSources will be pooled using
    // TaskPool<T>, so again, no garabage will be generated.
    //
    // Hence we achieve 0 memory allocation, making our tasks run really fast.

    /// <summary>
    /// GDTaskSource that has a void return (returns nothing).
    /// </summary>
    public interface IGdTaskSource
    {
        GdTaskStatus GetStatus(short token);
        void OnCompleted(Action<object> continuation, object state, short token);
        void GetResult(short token);

        GdTaskStatus UnsafeGetStatus(); // only for debug use.
    }

    /// <summary>
    /// GDTaskSource that has a return value of <see cref="T"/>
    /// </summary>
    /// <typeparam name="T">Return value of the task source</typeparam>
    public interface IGdTaskSource<out T> : IGdTaskSource
    {
        // Hide the original void GetResult method
        new T? GetResult(short token);
    }

    // Extensions are all aggressive inlined so all calls are substituted with raw code for greatest performance.
    public static class GdTaskStatusExtensions
    {
        /// <summary>status != Pending.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCompleted(this GdTaskStatus status)
        {
            return status != GdTaskStatus.Pending;
        }

        /// <summary>status == Succeeded.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCompletedSuccessfully(this GdTaskStatus status)
        {
            return status == GdTaskStatus.Succeeded;
        }

        /// <summary>status == Canceled.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCanceled(this GdTaskStatus status)
        {
            return status == GdTaskStatus.Canceled;
        }

        /// <summary>status == Faulted.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFaulted(this GdTaskStatus status)
        {
            return status == GdTaskStatus.Faulted;
        }
    }
}