using HVWpfScreenHelper;
using System.Collections.ObjectModel;

namespace TestBrightness.Model
{
	internal class MainData : NotifyPropertyChanged
	{
		private ObservableCollection<MonitorSettings> _monitorSettings = new ObservableCollection<MonitorSettings>();
		private int wmiMonitorBrightness = -1;

		public ObservableCollection<MonitorSettings> MonitorSettings { get => _monitorSettings; set => SetField(ref _monitorSettings, value); }

		public int WmiMonitorBrightness
		{
			get => wmiMonitorBrightness;
			set
			{
				SetField(ref wmiMonitorBrightness, value);
				NotifyPropertyChanges(nameof(WmiMonitorBrightnessSet));
			}
		}

		public bool WmiMonitorBrightnessSet { get => WmiMonitorBrightness != -1; }
	}
}