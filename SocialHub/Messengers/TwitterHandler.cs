using SocialBar.EventArgs;
using SocialBar.Interfaces;
using SocialBar.Wrappers;
using System.Configuration;
using System.Timers;

namespace SocialBar.Messengers
{
	public class TwitterHandler : Messenger
	{
		public string Name { get; set; }
		public string Message { get; set; }
		public string Title { get; set; }
		public string Sender { get; set; }
		private CustomTwitterClient client;
		public MessengerHandler Handler { get; set; }

		public TwitterHandler()
		{
			Name = "Twitter";
			var token = ConfigurationManager.AppSettings["TwitterToken"];
			var secret = ConfigurationManager.AppSettings["TwitterSecret"];
			client = new CustomTwitterClient(token, secret);
			client.OnNewTweet += OnNewTweet;
		}

		private void OnNewTweet(object sender, OnNewTweetArgs e)
		{
			Handler.TriggerAction(Name, e.Content, e.SenderUsername);
		}

		public void Run()
		{
			Timer t = new Timer(60000);
			t.Elapsed += OnElapsed;
			t.Enabled = true;
			client.Refresh();
		}

		private void OnElapsed(object sender, ElapsedEventArgs e)
		{
			client.Refresh();
		}
	}
}