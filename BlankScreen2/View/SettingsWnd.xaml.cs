using System.Windows;
using BlankScreen2.Helpers;

namespace BlankScreen2.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public sealed partial class SettingsWnd : Window
	{
		private ScreenMgr _ScreenMgr;

		public SettingsWnd(ScreenMgr screenMgr)
		{
			_ScreenMgr = screenMgr;
			InitializeComponent();
			this.DataContext = _ScreenMgr.Settings;
		}

		private void RefreshDisplays_Click(object sender, RoutedEventArgs e)
		{
			_ScreenMgr.RefreshDisplays();
		}

		private void BlanksScreen_Click(object sender, RoutedEventArgs e)
		{
			_ScreenMgr.ShowBlankScreen();
			this.Close();
		}
	}
}