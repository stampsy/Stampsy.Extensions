using System;
using System.Threading.Tasks;

namespace Stampsy.Extensions.UIKit
{
    public static class UIKitExtensions
    {
        /// <summary>
        /// Makes network indicator visible until <paramref name="task"/> is completed.
        /// If <paramref name="task"/> is already completed, nothing happens.
        /// </summary>
        public static Task WithNetworkIndicator (this Task task)
        {
            NetworkIndicator.RegisterTask (task);
            return task;
        }

        /// <summary>
        /// Makes network indicator visible until <paramref name="task"/> is completed.
        /// If <paramref name="task"/> is already completed, nothing happens.
        /// </summary>
        public static Task<T> WithNetworkIndicator<T> (this Task<T> task)
        {
            NetworkIndicator.RegisterTask (task);
            return task;
        }

        /// <summary>
        /// Asks iOS to avoid freezing your app while <paramref name="task"/> is running.
        /// Note that it's up to iOS to decide whether to honor your request.
        /// </summary>
        /// <remarks>
        /// Read more iOS background tasks:
        /// http://developer.apple.com/library/ios/DOCUMENTATION/UIKit/Reference/UIApplication_Class/Reference/Reference.html#//apple_ref/occ/instm/UIApplication/beginBackgroundTaskWithExpirationHandler:
        /// http://stackoverflow.com/a/12074879/458193
        /// </remarks>
        public static Task RegisterAsBackgroundTask (this Task task)
        {
            var bg = new BackgroundTaskHandle ();
            task.ContinueWith (t => bg.Dispose (), TaskContinuationOptions.ExecuteSynchronously);

            return task;
        }

        /// <summary>
        /// Asks iOS to avoid freezing your app while <paramref name="task"/> is running.
        /// Note that it's up to iOS to decide whether to honor your request.
        /// </summary>
        /// <remarks>
        /// Read more iOS background tasks:
        /// http://developer.apple.com/library/ios/DOCUMENTATION/UIKit/Reference/UIApplication_Class/Reference/Reference.html#//apple_ref/occ/instm/UIApplication/beginBackgroundTaskWithExpirationHandler:
        /// http://stackoverflow.com/a/12074879/458193
        /// </remarks>
        public static Task<T> RegisterAsBackgroundTask<T> (this Task<T> task)
        {
            var bg = new BackgroundTaskHandle ();
            task.ContinueWith (t => bg.Dispose (), TaskContinuationOptions.ExecuteSynchronously);

            return task;
        }
    }
}