using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MonoTouch;
using MonoTouch.Foundation;

namespace Stampsy.Extensions.Foundation
{
    /// <summary>
    /// Extensions to MonoTouch.Foundation.
    /// </summary>
    public static class FoundationExtensions
    {
        [DllImport (Constants.ObjectiveCLibrary)]
        static extern IntPtr object_getClassName (IntPtr obj);

        /// <summary>
        /// Returns Objective C class name for a given object.
        /// </summary>
        public static string GetClassName (this NSObject o) {
            return Marshal.PtrToStringAuto (object_getClassName (o.Handle));
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
    }
}

