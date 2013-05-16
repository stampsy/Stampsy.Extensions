using System;
using MonoTouch.UIKit;
using System.Threading;
using System.Threading.Tasks;

namespace Stampsy.Extensions.UIKit
{
    public class NetworkIndicator
    {
        static int _counter;

        static bool IsBusy {
            get { return _counter > 0; }
        }

        static void RefreshIndicator ()
        {
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = IsBusy;
        }

        /// <summary>
        /// Marks an activity as started and forces the network indicator to appear.
        /// Each <c>BeginActivity</c> call needs a matching <see cref="EndActivity"/> call.
        /// </summary>
        public static void BeginActivity ()
        {
            Interlocked.Increment (ref _counter);
            RefreshIndicator ();
        }

        /// <summary>
        /// Marks an activity previously started with <see cref="BeginActivity"/> as finished.
        /// When all activities are finished, the network indicator disappears.
        /// </summary>
        public static void EndActivity ()
        {
            Interlocked.Decrement (ref _counter);
            RefreshIndicator ();
        }

        /// <summary>
        /// Makes network indicator visible until <paramref name="task"/> is completed.
        /// If <paramref name="task"/> is already completed, nothing happens.
        /// </summary>
        public static void RegisterTask (Task task)
        {
            if (task.IsCompleted)
                return;

            BeginActivity ();

            task.ContinueWith (t => {
                EndActivity ();
            }, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}