using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SocialBar.Model
{
	/// <summary>
	/// Interaction logic for Notification.xaml
	/// </summary>
	public partial class Notification : ContentControl
	{
		public bool IsClosing { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }

		public Notification(NotificationContent c)
		{
			Color backgroundColor = (Color)ColorConverter.ConvertFromString(c.BackgroundColor);
			Color foregroundColor = (Color)ColorConverter.ConvertFromString(c.ForegroundColor);
			SolidColorBrush notificationBackgroundBrush = new SolidColorBrush(backgroundColor);
			SolidColorBrush notificationForegroundBrush = new SolidColorBrush(foregroundColor);
			ImageSource notifactionImage = new BitmapImage(new Uri(c.ImageUrl, UriKind.RelativeOrAbsolute));

			Console.WriteLine($"{c.Title}\n{c.Message}");
			InitializeComponent();
			NotificationGrid.Background = notificationBackgroundBrush;
			TitleBox.Content = c.Title;
			TitleBox.Foreground = notificationForegroundBrush;
			TitleBox.BorderBrush = notificationForegroundBrush;
			ContentBox.Text = c.Message;
			ContentBox.Foreground = notificationForegroundBrush;
			NotificationIcon.Source = notifactionImage;
		}

		public static readonly RoutedEvent NotificationClosedEvent = EventManager.RegisterRoutedEvent(
			"NotificationClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Notification));

		public event RoutedEventHandler NotificationClosed
		{
			add { AddHandler(NotificationClosedEvent, value); }
			remove { RemoveHandler(NotificationClosedEvent, value); }
		}

		public async void Close()
		{
			if (IsClosing)
			{
				return;
			}

			IsClosing = true;

			RaiseEvent(new RoutedEventArgs(NotificationClosedEvent));
		}
	}
}