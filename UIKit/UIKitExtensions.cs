using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace Stampsy.Extensions.UIKit
{
    public static class UIKitExtensions
    {
        [DllImport (Constants.ObjectiveCLibrary)]
        static extern IntPtr object_getClassName (IntPtr obj);

        [DllImport (Constants.SystemLibrary)]
        static extern int sysctlbyname ([MarshalAs(UnmanagedType.LPStr)] string property, IntPtr output, IntPtr oldLen, IntPtr newp, uint newlen);

        const string HardwareProperty = "hw.machine";

        /// <summary>
        /// Returns Objective C class name for a given object.
        /// </summary>
        public static string GetClassName (this NSObject o) {
            return Marshal.PtrToStringAuto (object_getClassName (o.Handle));
        }

        /// <summary>
        /// Returns a platform identifier, such as "iPhone5,2", "iPad2,5" or "x86_64".
        /// </summary>
        /// <remarks>
        /// See an up-to-date list of identifiers and platforms they correspond to:
        /// http://stackoverflow.com/a/3950748/458193
        /// </remarks>
        public static string GetPlatformIdentifier (this UIDevice device)
        {
            var pLen = Marshal.AllocHGlobal (sizeof (int));
            sysctlbyname (HardwareProperty, IntPtr.Zero, pLen, IntPtr.Zero, 0);

            var length = Marshal.ReadInt32 (pLen);

            var pStr = Marshal.AllocHGlobal (length);
            sysctlbyname (HardwareProperty, pStr, pLen, IntPtr.Zero, 0);

            return Marshal.PtrToStringAnsi (pStr);
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

        /// <summary>
        /// Scales the image up or down without preserving alpha channel.
        /// This method is slightly faster than <see cref="UIImage.Scale"/>.
        /// </summary>
        public static UIImage ScaleOpaque (this UIImage img, SizeF newSize, float scale = 0)
        {
            UIGraphics.BeginImageContextWithOptions (newSize, true, scale);
            img.Draw (new RectangleF (0f, 0f, newSize.Width, newSize.Height));
            UIImage imageFromCurrentImageContext = UIGraphics.GetImageFromCurrentImageContext ();
            UIGraphics.EndImageContext ();

            return imageFromCurrentImageContext;
        }

        /// <summary>
        /// Creates an index path from <paramref name="index"/>.
        /// </summary>
        public static NSIndexPath ToRow (this int index)
        {
            return NSIndexPath.FromRowSection (index, 0);
        }

        /// <summary>
        /// Creates a single-element index path array from <paramref name="index"/>.
        /// </summary>
        public static NSIndexPath [] ToRows (this int index)
        {
            return new [] { index.ToRow () };
        }

        /// <summary>
        /// Creates an index path array from a sequence of indices.
        /// </summary>
        public static NSIndexPath [] ToRows (this IEnumerable<int> indices)
        {
            return indices.Select (ToRow).ToArray ();
        }

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
    }
}