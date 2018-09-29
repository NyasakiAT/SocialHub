using SocialBar.Interfaces;
using System;
using System.Configuration;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace SocialBar.Messengers
{
	internal class TwitchHandler : Messenger
	{
		private TwitchClient client;
		public string Name { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public string Sender { get; set; }

		public string Username { get; set; }
		public string Token { get; set; }
		public string Channel { get; set; }

		public MessengerHandler Handler { get; set; }

		public TwitchHandler()
		{
			Name = "Twitch";
			Username = ConfigurationManager.AppSettings["TwitchUsername"];
			Token = ConfigurationManager.AppSettings["TwitchToken"];
			Channel = ConfigurationManager.AppSettings["TwitchChannel"];

			ConnectionCredentials credentials = new ConnectionCredentials(Username, Token);

			client = new TwitchClient();
			client.Initialize(credentials, Channel);

			client.OnJoinedChannel += onJoinedChannel;
			client.OnMessageReceived += onMessageReceived;
			client.OnWhisperReceived += onWhisperReceived;
			client.OnConnected += Client_OnConnected;
		}

		public void Run()
		{
			client.Connect();
		}

		private void Client_OnConnected(object sender, OnConnectedArgs e)
		{
			Console.WriteLine("Im connected");
		}

		private void onJoinedChannel(object sender, OnJoinedChannelArgs e)
		{
			Console.WriteLine("Ready now :)");
			Handler.TriggerAction("Twitch", $"Connected with {Username} to channel {Channel} :)", "SocialHub");
		}

		private void onMessageReceived(object sender, OnMessageReceivedArgs e)
		{
			if (Sender != client.TwitchUsername)
				Handler.TriggerAction(Name, e.ChatMessage.Message, e.ChatMessage.Username, e.ChatMessage.Channel);
		}

		private void onWhisperReceived(object sender, OnWhisperReceivedArgs e)
		{
			if (Sender != client.TwitchUsername)
				Handler.TriggerAction(Name, e.WhisperMessage.Message, e.WhisperMessage.Username, "Whisper");
		}
	}
}