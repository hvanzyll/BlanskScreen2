namespace HVWpfScreenHelper
{
	public class BrightnessSettings : NotifyPropertyChanged
	{
		private bool _settingsSet;
		private int _minimumBrightness;
		private int _maximumBrightness;
		private int _currentBrightness;

		public bool settingsSet { get; private set; } = false;
		public int MinimumBrightness { get => _minimumBrightness; set => SetField(ref _minimumBrightness, value); }
		public int MaximumBrightness { get => _maximumBrightness; set => SetField(ref _maximumBrightness, value); }
		public int CurrentBrightness { get => _currentBrightness; set => SetField(ref _currentBrightness, value); }
		public bool SettingsSet { get => _settingsSet; private set => SetField(ref _settingsSet, value); }

		public void SetSettingsSet()
		{
			settingsSet = true;
		}
	}

	public class ContrastSettings
	{
		private bool settingsSet { get; set; } = false;
		public int MinimumContrast { get; set; } = 0;
		public int CurrentContrast { get; set; } = 0;
		public int MaximumContrast { get; set; } = 0;

		public void SettingsSet()
		{
			settingsSet = true;
		}

		public bool AreSettingsSet()
		{
			return settingsSet;
		}
	}
}