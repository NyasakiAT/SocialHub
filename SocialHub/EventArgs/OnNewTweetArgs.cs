using System;
using System.Collections.Generic;
using Tweetinvi.Models.Entities;

namespace SocialBar.EventArgs
{
	public class OnNewTweetArgs
	{
		public String SenderUsername { get; set; }
		public String Content { get; set; }
		public List<IMediaEntity> MediaUrls { get; set; } = new List<IMediaEntity>();

		public OnNewTweetArgs()
		{
		}
	}
}