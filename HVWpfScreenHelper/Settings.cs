namespace HVWpfScreenHelper
{
	public class BrightnessSettings
	{
		private bool settingsSet { get; set; } = false;
		public int MinimumBrightness { get; set; } = 0;
		public int MaximumBrightness { get; set; } = 0;
		public int CurrentBrightness { get; set; } = 0;

		public void SettingsSet()
		{
			settingsSet = true;
		}

		public bool AreSettingsSet()
		{
			return settingsSet;
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