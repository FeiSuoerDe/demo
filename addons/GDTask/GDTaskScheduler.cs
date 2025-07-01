using System;
using Godot;

namespace demo.addons.GDTask
{
    // GDTask has no scheduler like TaskScheduler.
    // Only handle unobserved exception.

    public static class GdTaskScheduler
    {
        public static event Action<Exception> UnobservedTaskException;

        /// <summary>
        /// Propagate OperationCanceledException to UnobservedTaskException when true. Default is false.
        /// </summary>
        public static bool PropagateOperationCanceledException = false;

        internal static void PublishUnobservedTaskException(Exception ex)
        {
            if (ex != null)
            {
                if (!PropagateOperationCanceledException && ex is OperationCanceledException)
                {
                    return;
                }

                if (UnobservedTaskException != null)
                {
                    UnobservedTaskException.Invoke(ex);
                }
                else
                {
                    GD.PrintErr("UnobservedTaskException: " + ex.ToString());
                }
            }
        }
    }
}

