using System.Windows;

namespace SocialBar.View
{
	/// <summary>
	/// Interaction logic for TwitterPinDialog.xaml
	/// </summary>
	public partial class TwitchPinDialog : Window
	{
		public string PIN
		{
			get { return pinInput.Text; }
		}

		public TwitchPinDialog()
		{
			InitializeComponent();
		}

		private void ConfirmClicked(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
	}
}