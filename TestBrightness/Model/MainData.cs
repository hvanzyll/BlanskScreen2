using HVWpfScreenHelper;
using System.Collections.ObjectModel;

namespace TestBrightness.Model
{
	internal class MainData : NotifyPropertyChanged
	{
		private ObservableCollection<MonitorSettings> _monitorSettings = new ObservableCollection<MonitorSettings>();

		public ObservableCollection<MonitorSettings> MonitorSettings { get => _monitorSettings; set => SetField(ref _monitorSettings, value); }
	}
}