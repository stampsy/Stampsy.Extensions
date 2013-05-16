using System;
using MonoTouch.UIKit;
using System.Threading;
using System.Threading.Tasks;

namespace Stampsy.Extensions.UIKit
{
    public class NetworkIndicator
    {
        static int _counter;

        public static bool IsBusy {
            get { return _counter > 0; }
        }

        public static void BeginActivity ()
        {
            Interlocked.Increment (ref _counter);
            RefreshIndicator ();
        }

        public static void EndActivity ()
        {
            Interlocked.Decrement (ref _counter);
            RefreshIndicator ();
        }

        static void RefreshIndicator ()
        {
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = IsBusy;
        }

        public static void RegisterTask (Task task)
        {
            if (task.IsCanceled || task.IsFaulted)
                return;

            BeginActivity ();

            task.ContinueWith (t => {
				EndActivity ();
			}, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}

