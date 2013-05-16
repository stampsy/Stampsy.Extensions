using System;
using System.Threading.Tasks;

namespace Stampsy.Extensions.UIKit
{
	public static class UIKitExtensions
	{
		public static Task WithNetworkIndicator (this Task task)
		{
			NetworkIndicator.RegisterTask (task);
			return task;
		}

		public static Task<T> WithNetworkIndicator<T> (this Task<T> task)
		{
			NetworkIndicator.RegisterTask (task);
			return task;
		}
	}
}