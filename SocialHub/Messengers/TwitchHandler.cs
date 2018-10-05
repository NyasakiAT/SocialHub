using SocialBar.EventArgs;
using SocialBar.Interfaces;
using SocialBar.View;
using SocialBar.Wrappers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace SocialBar.Messengers
{
	public class TwitchHandler : Messenger
	{
		private CustomTwitchClient client;
		public string Name { get; set; }

		public string Title { get; set; }
		public string Message { get; set; }
		public string Sender { get; set; }

		public string Username { get; set; }
		public string Token { get; set; }
		public string Channel { get; set; }

		public MessengerHandler Handler { get; set; }

		private List<string> whisperList = new List<string>();
		private bool whisperDeactivated = true;

		public TwitchHandler()
		{
			try
			{
				Name = "Twitch";
				Username = ConfigurationManager.AppSettings["TwitchUsername"];
				Token = ConfigurationManager.AppSettings["TwitchToken"];
				Channel = ConfigurationManager.AppSettings["TwitchChannel"];
				whisperDeactivated = Boolean.Parse(ConfigurationManager.AppSettings["DisableWhisper"]);

				if (Token == "")
				{
					TwitchPinDialog inputDialog = new TwitchPinDialog();
					string pinCode = null;

					Process.Start("https://twitchtokengenerator.com/");

					if (inputDialog.ShowDialog() == true)
						pinCode = inputDialog.PIN;

					var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
					var settings = configFile.AppSettings.Settings;

					settings["TwitchToken"].Value = pinCode;

					configFile.Save(ConfigurationSaveMode.Modified);
					ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("Error on parsing DisableWhisper to boolean", "Error");
			}

			ConnectionCredentials credentials = new ConnectionCredentials(Username, Token);

			client = new CustomTwitchClient();
			client.TwitchUser = Username;
			client.Initialize(credentials, Channel);
			client.OnJoinedChannel += OnJoinedChannel;
			client.OnMessageReceived += OnMessageReceived;
			client.OnWhisperReceived += OnWhisperReceived;
			client.OnConnected += OnConnected;
			client.OnStreamingStateChanged += OnStreamingStateChanged;
		}

		private void OnStreamingStateChanged(object sender, OnStreamingStateChangedArgs e)
		{
			if (e.IsStreaming)
				Handler.TriggerAction(Name, $"{Username} is streaming now!", "Twitch");
			else
				Handler.TriggerAction(Name, $"{Username} has stopped streaming :(", "Twitch");
		}

		public void Run()
		{
			client.Connect();
		}

		private void OnConnected(object sender, OnConnectedArgs e)
		{
			Console.WriteLine("Im connected");
		}

		private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
		{
			Console.WriteLine("Ready now :)");
			Handler.TriggerAction("Twitch", $"Connected with {Username} to channel {Channel} :)", "SocialHub");
		}

		private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
		{
			if (Sender != client.TwitchUsername)
				Handler.TriggerAction(Name, e.ChatMessage.Message, e.ChatMessage.Username, e.ChatMessage.Channel);
		}

		private void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
		{
			if (Sender != client.TwitchUsername && !whisperDeactivated)
			{
				Handler.TriggerAction(Name, $"from {e.WhisperMessage.Username}", "Whisper", "Whisper");
			}
		}
	}
}