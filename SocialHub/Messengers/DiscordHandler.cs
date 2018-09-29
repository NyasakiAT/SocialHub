using Discord;
using Discord.WebSocket;
using SocialBar.Interfaces;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

		public DiscordHandler()
		{
			Name = "Discord";
			worker = new BackgroundWorker();
			worker.DoWork += doWork;
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
			Message = message.Content; //TODO REMOVE
			Title = message.Channel.Name; //!!!
			Sender = message.Author.Username; //!!!
			if (Sender != client.CurrentUser.Username)
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