using HVWpfScreenHelper;
using System.Collections.Generic;
using System.Threading.Tasks;
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

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			await Task.Run(() =>
			{
				IEnumerable<Screen> allScreens = Screen.AllScreens;

				foreach (Screen screen in allScreens)
				{
					Dispatcher.Invoke(() => _MainData.MonitorSettings.Add(screen.MonitorCapabilites));
				};

				Parallel.ForEach(allScreens, screen =>
				{
					screen.RefreshMonitorCapabilities();
					screen.BackupMonitorCapabilities();
				});
			});
		}
	}
}