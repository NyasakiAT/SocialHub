using SocialBar.Interfaces;
using SocialBar.Messengers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SocialBar.Model
{
	public class MessageHandler : MessengerHandler, INotifyPropertyChanged
	{
		private string _history = "";
		public String History { get { return _history; } set { _history = value; NotifyPropertyChanged(); } }
		public List<Messenger> Messengers { get; set; } = new List<Messenger>();
		private MainWindow _w;

		public MessageHandler(MainWindow w)
		{
			_w = w;
			DiscordHandler d = new DiscordHandler();
			TwitchHandler t = new TwitchHandler();
			Messengers.Add(d);
			Messengers.Add(t);
			RunMessengers();
		}

		private void RunMessengers()
		{
			foreach (var messenger in Messengers)
			{
				messenger.Handler = this;
				messenger.Run();
			}
		}

		public void TriggerAction(String messenger, String message, String sender, String title = "")
		{
			History += $"{messenger} -> {title}\nMessage from {sender}\n{message}\n\n";
			var notificationManager = new NotificationHandler(_w);
			string backgroundAsString;
			string foregroundAsString;
			string imageUrl;

			switch (messenger)
			{
				case "Discord":
					backgroundAsString = "#7289DA";
					foregroundAsString = "#000000";
					imageUrl = AppDomain.CurrentDomain.BaseDirectory + @"IMG\discord.png";
					break;

				case "Twitch":
					backgroundAsString = "#4B367C";
					foregroundAsString = "#fff";
					imageUrl = AppDomain.CurrentDomain.BaseDirectory + @"IMG\twitch.png";
					break;

				default:
					backgroundAsString = "#ffa0fa";
					foregroundAsString = "#000000";
					imageUrl = AppDomain.CurrentDomain.BaseDirectory + @"";
					break;
			}

			notificationManager.Show(new NotificationContent
			{
				Title = $"{messenger} from {sender}",
				Message = message,
				BackgroundColor = backgroundAsString,
				ForegroundColor = foregroundAsString,
				ImageUrl = imageUrl,
			});
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