using System;

namespace SocialBar.EventArgs
{
	public class OnStreamingStateChangedArgs
	{
		public String Username { get; set; }
		public bool IsStreaming { get; set; }

		public OnStreamingStateChangedArgs()
		{
		}
	}
}