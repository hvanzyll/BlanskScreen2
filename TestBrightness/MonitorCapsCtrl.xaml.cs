using HVWpfScreenHelper;
using System.Windows.Controls;

namespace TestBrightness
{
	/// <summary>
	/// Interaction logic for MonitorCapsCtrl.xaml
	/// </summary>
	public partial class MonitorCapsCtrl : UserControl
	{
		public MonitorCapsCtrl()
		{
			InitializeComponent();
		}

		private void BrightnessSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.DataContext is not MonitorSettings monitorSettings)
				return;

			monitorSettings.Screen?.SetBrightness(monitorSettings.BrightnessSettings.CurrentBrightness);
		}

		private void ConstrastSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.DataContext is not MonitorSettings monitorSettings)
				return;

			monitorSettings.Screen?.SetContrast(monitorSettings.ContrastSettings.CurrentContrast);
		}
	}
}