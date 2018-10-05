using SocialBar.EventArgs;
using SocialBar.Parameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using TwitchLib.Api;
using TwitchLib.Client;

namespace SocialBar.Wrappers
{
	public class CustomTwitchClient : TwitchClient
	{
		private TwitchAPI api;
		private bool initRun = true;
		public String TwitchUser { get; set; }
		public Dictionary<string, bool> Follows = new Dictionary<string, bool>();

		public CustomTwitchClient()
		{
			api = new TwitchAPI();
			api.Settings.ClientId = ApiTokens.TWITCH_KEY;
			api.Settings.Secret = ApiTokens.TWITCH_SECRET;
			Timer t = new Timer(60000);
			t.Elapsed += RefreshStats;
			t.Enabled = true;
		}

		private void RefreshStats(object sender, ElapsedEventArgs e)
		{
			MainAsync().GetAwaiter().GetResult();
		}

		#region API Events

		private async Task MainAsync()
		{
			GetIsStreaming();
		}

		public async Task GetIsStreaming()
		{
			var Users = await api.V5.Users.GetUserByNameAsync(TwitchUser);
			var userFollows = await api.V5.Users.GetUserFollowsAsync(Users.Matches[0].Id);
			foreach (var item in userFollows.Follows)
			{
				bool isStreaming = await api.V5.Streams.BroadcasterOnlineAsync(item.Channel.Id);

				if (initRun)
				{
					if (!Follows.ContainsKey(item.Channel.Name.ToString()))
					{
						Console.WriteLine("Adding: " + item.Channel.Name.ToString() + ", " + false);
						Follows.Add(item.Channel.Name.ToString(), false);
					}
				}
				else
				{
					if (Follows[item.Channel.Name.ToString()] != isStreaming)
					{
						StreamingStateChanged(new OnStreamingStateChangedArgs() { Username = item.Channel.Name.ToString(), IsStreaming = isStreaming });
						Follows[item.Channel.Name.ToString()] = isStreaming;
					}
				}
			}
			initRun = false;
		}

		public event EventHandler<OnStreamingStateChangedArgs> OnStreamingStateChanged;

		protected virtual void StreamingStateChanged(OnStreamingStateChangedArgs e)
		{
			EventHandler<OnStreamingStateChangedArgs> handler = OnStreamingStateChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		#endregion API Events
	}
}