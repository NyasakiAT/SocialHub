using SocialBar.Model;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SocialBar.ViewModel
{
	/// <summary>
	/// Main viewmodel for the application
	/// </summary>
	public class Main : INotifyPropertyChanged
	{
		private MessageHandler _handler;
		public MessageHandler MsgHandler { get { return _handler; } set { _handler = value; } }

		public Main(MainWindow w)
		{
			MsgHandler = new MessageHandler(w);
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