using Discord;
using Discord.WebSocket;
using SocialBar.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace SocialBar.Model
{
	public class DiscordHandler : INotifyPropertyChanged, Messenger
	{
		private string _message;
		private string _title;
		private string _sender;
		private DiscordSocketClient client;
		private BackgroundWorker worker;

		public String Message { get { return _message; } set { _message = value; NotifyPropertyChanged(); } }

		public String Title { get { return _title; } set { _title = value; NotifyPropertyChanged(); } }
		public String Sender { get { return _sender; } set { _sender = value; NotifyPropertyChanged(); } }

		public string Name { get; set; }
		public MessengerHandler Handler { get; set; }

		private bool whisperOnly = false;

		public DiscordHandler()
		{
			Name = "Discord";
			worker = new BackgroundWorker();
			worker.DoWork += doWork;

			try
			{
				whisperOnly = Boolean.Parse(ConfigurationManager.AppSettings["WhisperOnly"]);
			}
			catch (Exception e)
			{
				MessageBox.Show("Error on parsing WhisperOnly to boolean", "Error");
			}
		}

		public async Task MainAsync()
		{
			client = new DiscordSocketClient();

			string token = ConfigurationManager.AppSettings["DiscordKey"];
			client.MessageReceived += MessageReceived;
			client.Connected += ClientConnected;
			await client.LoginAsync(TokenType.User, token);
			await client.StartAsync();

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private async Task ClientConnected()
		{
			Handler.TriggerAction("Discord", $"Connected with {client.CurrentUser.Username} :)", "SocialHub");
		}

		/// <summary>
		/// OnMessageRecieved event
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private async Task MessageReceived(SocketMessage message)
		{
			String serverName = "NoServer";

			if (!message.Channel.Name.StartsWith("@"))
			{
				List<SocketGuild> x = new List<SocketGuild>();
				foreach (var item in client.Guilds)
				{
					try
					{
						var searched = item.GetChannel(message.Channel.Id).Name;
						if (searched == message.Channel.Name)
							serverName = item.Name;
					}
					catch (Exception)
					{
						Console.WriteLine($"Channel not in {item.Name}");
					}
				}
			}

			Console.WriteLine(serverName);

			//Needed for discord to work

			Title = message.Channel.Name;
			Message = message.Content;

			if (serverName != "NoServer")
				Sender = message.Author.Username + $" @ {serverName} / {message.Channel.Name}\n";
			else
				Sender = message.Author.Username;
			//-----

			Console.WriteLine(message.Channel.Name);

			if (whisperOnly)
			{
				if (message.Channel.Name.StartsWith("@"))
				{
					if (Sender != client.CurrentUser.Username)
						Handler.TriggerAction(Name, Message, Sender, Title);
				}
			}
			else
				//if (Sender != client.CurrentUser.Username)
				Handler.TriggerAction(Name, Message, Sender, Title);
		}

		/// <summary>
		/// Used to run the plugin in a different thread
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void doWork(object sender, DoWorkEventArgs e)
		{
			MainAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// Method which activates/runs the plugin
		/// </summary>
		public void Run()
		{
			worker.RunWorkerAsync();
		}

		#region PropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion PropertyChanged
	}
}