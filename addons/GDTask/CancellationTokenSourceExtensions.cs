using System;
using System.Threading;
using Godot;
using TimelapseInvoices.addons.GDTask.Autoload;
using TimelapseInvoices.addons.GDTask.Triggers;

namespace TimelapseInvoices.addons.GDTask
{

    public static partial class CancellationTokenSourceExtensions
    {
        private readonly static Action<object>? CancelCancellationTokenSourceStateDelegate = new Action<object>(CancelCancellationTokenSourceState);

        private static void CancelCancellationTokenSourceState(object state)
        {
            var cts = (CancellationTokenSource)state;
            cts.Cancel();
        }

        public static IDisposable CancelAfterSlim(this CancellationTokenSource cts, int millisecondsDelay, DelayType delayType = DelayType.DeltaTime, PlayerLoopTiming delayTiming = PlayerLoopTiming.Process)
        {
            return CancelAfterSlim(cts, TimeSpan.FromMilliseconds(millisecondsDelay), delayType, delayTiming);
        }

        public static IDisposable CancelAfterSlim(this CancellationTokenSource cts, TimeSpan delayTimeSpan, DelayType delayType = DelayType.DeltaTime, PlayerLoopTiming delayTiming = PlayerLoopTiming.Process)
        {
            return PlayerLoopTimer.StartNew(delayTimeSpan, false, delayType, delayTiming, cts.Token, CancelCancellationTokenSourceStateDelegate, cts);
        }

        public static void RegisterRaiseCancelOnDestroy(this CancellationTokenSource cts, Node node)
        {
            var trigger = node.GetAsyncDestroyTrigger();
            trigger.CancellationToken.RegisterWithoutCaptureExecutionContext(CancelCancellationTokenSourceStateDelegate, cts);
        }
    }
}

