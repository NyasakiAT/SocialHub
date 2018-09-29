using SocialBar.Model;
using SocialBar.ViewModel;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace SocialBar
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private IList _items;
		private int maxItems = 5;
		private NotifyIcon notifyIcon = null;

		public MainWindow()
		{
			notifyIcon = new NotifyIcon();

			notifyIcon.Icon = new System.Drawing.Icon(@"IMG\app.ico"); //.ico file needed
			notifyIcon.Click += new EventHandler(notifyIcon_Click);

			InitializeComponent();
			MainViewModel vm = new MainViewModel(this);
			this.DataContext = vm;

			_items = NotificationPanel.Children;

			var workArea = SystemParameters.WorkArea;

			this.Left = workArea.Left;
			this.Top = workArea.Top;
			this.Width = workArea.Width;
			this.Height = workArea.Height;
		}

		public async void ShowNotification(object content, TimeSpan expirationTime)
		{
			var notification = new Notification((NotificationContent)content);

			notification.NotificationClosed += OnNotificationClosed;

			if (!IsLoaded)
			{
				return;
			}

			lock (_items)
			{
				_items.Add(notification);

				if (_items.OfType<Notification>().Count(i => !i.IsClosing) > maxItems)
				{
					_items.OfType<Notification>().First(i => !i.IsClosing).Close();
				}
			}

			if (expirationTime == TimeSpan.MaxValue)
			{
				return;
			}
			await Task.Delay(expirationTime);
			notification.Close();
		}

		private void notifyIcon_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void OnNotificationClosed(object sender, RoutedEventArgs routedEventArgs)
		{
			var notification = sender as Notification;
			_items.Remove(notification);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			notifyIcon.Visible = true;
		}
	}
}