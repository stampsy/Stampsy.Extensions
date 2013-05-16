using System;
using MonoTouch.UIKit;

namespace Stampsy.Extensions.UIKit
{
    public class BackgroundTaskHandle : IDisposable
    {
        private int? _taskId;

        public BackgroundTaskHandle ()
        {
            _taskId = UIApplication.SharedApplication.BeginBackgroundTask (Dispose);
        }

        public void Dispose ()
        {
            UIApplication.SharedApplication.BeginInvokeOnMainThread (EndTask);
        }

        void EndTask ()
        {
            if (!_taskId.HasValue)
                return;

            UIApplication.SharedApplication.EndBackgroundTask (_taskId.Value);
            _taskId = null;
        }
    }
}