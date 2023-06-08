using HVWpfScreenHelper;
using System.Collections.Generic;
using System.Windows;
using TestBrightness.Model;

namespace TestBrightness
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private MainData _MainData = new MainData();

		public MainWindow()
		{
			InitializeComponent();

			this.DataContext = _MainData;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			IEnumerable<Screen> allScreens = Screen.AllScreens;
			foreach (Screen screen in allScreens)
			{
				screen.RefreshMonitorCapabilities();
				screen.BackupMonitorCapabilities();

				_MainData.MonitorSettings.Add(screen.MonitorCapabilites);
			}
		}
	}
}