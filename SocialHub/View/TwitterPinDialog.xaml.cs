using System.Windows;

namespace SocialBar.View
{
	/// <summary>
	/// Interaction logic for TwitterPinDialog.xaml
	/// </summary>
	public partial class TwitterPinDialog : Window
	{
		public string PIN
		{
			get { return pinInput.Text; }
		}

		public TwitterPinDialog()
		{
			InitializeComponent();
		}

		private void ConfirmClicked(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
	}
}