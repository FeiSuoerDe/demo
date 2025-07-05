using System;

namespace demo.addons.GDTask
{
    public abstract class MoveNextSource : IGdTaskSource<bool>
    {
        protected GdTaskCompletionSourceCore<bool> CompletionSource;

        public bool GetResult(short token)
        {
            return CompletionSource.GetResult(token);
        }

        public GdTaskStatus GetStatus(short token)
        {
            return CompletionSource.GetStatus(token);
        }

        public void OnCompleted(Action<object> continuation, object state, short token)
        {
            CompletionSource.OnCompleted(continuation, state, token);
        }

        public GdTaskStatus UnsafeGetStatus()
        {
            return CompletionSource.UnsafeGetStatus();
        }

        void IGdTaskSource.GetResult(short token)
        {
            CompletionSource.GetResult(token);
        }

        protected bool TryGetResult<T>(GdTask<T?>.Awaiter awaiter, out T? result)
        {
            try
            {
                result = awaiter.GetResult();
                return true;
            }
            catch (Exception? ex)
            {
                CompletionSource.TrySetException(ex);
                result = default;
                return false;
            }
        }

        protected bool TryGetResult(GDTask.Awaiter awaiter)
        {
            try
            {
                awaiter.GetResult();
                return true;
            }
            catch (Exception? ex)
            {
                CompletionSource.TrySetException(ex);
                return false;
            }
        }
    }
}
