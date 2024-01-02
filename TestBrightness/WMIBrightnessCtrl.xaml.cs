using System.Windows;
using System.Windows.Controls;
using TestBrightness.Model;
using WMIManagement;

namespace TestBrightness
{
	/// <summary>
	/// Interaction logic for WMIBrightnessCtrl.xaml
	/// </summary>
	public partial class WMIBrightnessCtrl : UserControl
	{
		public WMIBrightnessCtrl()
		{
			InitializeComponent();
			this.DataContextChanged += WMIBrightnessCtrl_DataContextChanged;
		}

		private void WMIBrightnessCtrl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (DataContext is not MainData mainData)
				return;

			MonitorBrightness.Get();

			if (!mainData.WmiMonitorBrightnessSet)
				return;

			MonitorBrightness.Set(mainData.WmiMonitorBrightness);
			mainData.WmiMonitorBrightness = MonitorBrightness.Get();
		}

		private void WMIBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (DataContext is not MainData mainData)
				return;

			if (!mainData.WmiMonitorBrightnessSet)
				return;

			MonitorBrightness.Set(mainData.WmiMonitorBrightness);
			mainData.WmiMonitorBrightness = MonitorBrightness.Get();
		}
	}
}