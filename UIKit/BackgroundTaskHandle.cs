using System;
using MonoTouch.UIKit;

namespace Stampsy.Extensions.UIKit
{
    /// <summary>
    /// Represents an iOS background task handle.
    /// </summary>
    /// <remarks>
    /// Create an instance of this class to let iOS know you're in the middle of an important operation
    /// and prefer your app to not get frozen if the user switches to another app.
    ///
    /// Dispose it when the operation is over and you don't mind iOS freezing the app.
    /// Note that it's up to iOS to decide whether to honor your request.
    ///
    /// Read more iOS background tasks:
    /// http://developer.apple.com/library/ios/DOCUMENTATION/UIKit/Reference/UIApplication_Class/Reference/Reference.html#//apple_ref/occ/instm/UIApplication/beginBackgroundTaskWithExpirationHandler:
    /// http://stackoverflow.com/a/12074879/458193
    /// </remarks>
    public class BackgroundTaskHandle : IDisposable
    {
        private int? _taskId;

        public BackgroundTaskHandle ()
        {
            _taskId = UIApplication.SharedApplication.BeginBackgroundTask (Dispose);
        }

        /// <summary>
        /// Dispose <see cref="BackgroundTaskHandle"/> to let iOS know that the important operation is over.
        /// It is also possible that iOS doesn't honor your request and still freezes your app.
        /// </summary>
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