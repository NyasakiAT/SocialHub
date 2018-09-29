using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SocialBar.Model
{
	public class NotificationHandler
	{
		private static List<StackPanel> Areas = new List<StackPanel>();
		private readonly Dispatcher _dispatcher;
		private MainWindow _w;
		private TimeSpan expirationTime;

		public NotificationHandler(MainWindow w, Dispatcher dispatcher = null)
		{
			_w = w;

			try
			{
				expirationTime = TimeSpan.FromSeconds(Int32.Parse(ConfigurationManager.AppSettings["Expirationtime"]));
			}
			catch (Exception e)
			{
				MessageBox.Show("Error on parsing Expirationtime to int", "Error");
			}

			if (dispatcher == null)
			{
				dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
			}
			_dispatcher = dispatcher;
		}

		public void Show(object content)
		{
			if (!_dispatcher.CheckAccess())
			{
				_dispatcher.BeginInvoke(
					new Action(() => Show(content)));
				return;
			}

			_w.ShowNotification(content, expirationTime);
		}

		internal static void AddArea(StackPanel area)
		{
			Areas.Add(area);
		}
	}
}