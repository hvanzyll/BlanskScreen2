using System.Collections.Generic;
using static HVWpfScreenHelper.NativeMethods;

namespace HVWpfScreenHelper
{
	public class MonitorSettings
	{
		private readonly Screen _screen;
		private NativeMethods.MonitorCapabilitiesMask monitorCababilities = MonitorCapabilitiesMask.MC_CAPS_NONE;
		private NativeMethods.ColorTemperatureMask colorTemperature = ColorTemperatureMask.MC_SUPPORTED_COLOR_TEMPERATURE_NONE;

		private readonly BrightnessSettings brightnessSettings = new BrightnessSettings();
		private readonly ContrastSettings contrastSettings = new ContrastSettings();

		internal MonitorCapabilitiesMask MonitorCababilities { get => monitorCababilities; set => monitorCababilities = value; }
		internal ColorTemperatureMask ColorTemperature { get => colorTemperature; set => colorTemperature = value; }

		//public bool MC_CAPS_NONE { get => (MonitorCapabilitiesMask.MC_CAPS_NONE & MonitorCababilities) == MonitorCapabilitiesMask.MC_CAPS_NONE; }
		public bool MC_CAPS_NONE { get => MonitorCababilities == MonitorCapabilitiesMask.MC_CAPS_NONE; }

		public bool MC_CAPS_MONITOR_TECHNOLOGY_TYPE { get => (MonitorCapabilitiesMask.MC_CAPS_MONITOR_TECHNOLOGY_TYPE & MonitorCababilities) == MonitorCapabilitiesMask.MC_CAPS_MONITOR_TECHNOLOGY_TYPE; }
		public bool MC_CAPS_BRIGHTNESS { get => (MonitorCapabilitiesMask.MC_CAPS_BRIGHTNESS & MonitorCababilities) == MonitorCapabilitiesMask.MC_CAPS_BRIGHTNESS; }
		public bool MC_CAPS_CONTRAST { get => (MonitorCapabilitiesMask.MC_CAPS_CONTRAST & MonitorCababilities) == MonitorCapabilitiesMask.MC_CAPS_CONTRAST; }
		public bool MC_CAPS_COLOR_TEMPERATURE { get => (MonitorCapabilitiesMask.MC_CAPS_COLOR_TEMPERATURE & MonitorCababilities) == MonitorCapabilitiesMask.MC_CAPS_COLOR_TEMPERATURE; }
		public bool MC_CAPS_RED_GREEN_BLUE_GAIN { get => (MonitorCapabilitiesMask.MC_CAPS_RED_GREEN_BLUE_GAIN & MonitorCababilities) == MonitorCapabilitiesMask.MC_CAPS_RED_GREEN_BLUE_GAIN; }
		public bool MC_CAPS_RED_GREEN_BLUE_DRIVE { get => (MonitorCapabilitiesMask.MC_CAPS_RED_GREEN_BLUE_DRIVE & MonitorCababilities) == MonitorCapabilitiesMask.MC_CAPS_RED_GREEN_BLUE_DRIVE; }
		public bool MC_CAPS_DEGAUSS { get => (MonitorCapabilitiesMask.MC_CAPS_DEGAUSS & MonitorCababilities) == MonitorCapabilitiesMask.MC_CAPS_DEGAUSS; }
		public bool MC_CAPS_DISPLAY_AREA_POSITION { get => (MonitorCapabilitiesMask.MC_CAPS_DISPLAY_AREA_POSITION & MonitorCababilities) == MonitorCapabilitiesMask.MC_CAPS_DISPLAY_AREA_POSITION; }
		public bool MC_CAPS_DISPLAY_AREA_SIZE { get => (MonitorCapabilitiesMask.MC_CAPS_DISPLAY_AREA_SIZE & MonitorCababilities) == MonitorCapabilitiesMask.MC_CAPS_DISPLAY_AREA_SIZE; }
		public bool MC_CAPS_RESTORE_FACTORY_DEFAULTS { get => (MonitorCapabilitiesMask.MC_CAPS_RESTORE_FACTORY_DEFAULTS & MonitorCababilities) == MonitorCapabilitiesMask.MC_CAPS_RESTORE_FACTORY_DEFAULTS; }
		public bool MC_CAPS_RESTORE_FACTORY_COLOR_DEFAULTS { get => (MonitorCapabilitiesMask.MC_CAPS_RESTORE_FACTORY_COLOR_DEFAULTS & MonitorCababilities) == MonitorCapabilitiesMask.MC_CAPS_RESTORE_FACTORY_COLOR_DEFAULTS; }
		public bool MC_RESTORE_FACTORY_DEFAULTS_ENABLES_MONITOR_SETTINGS { get => (MonitorCapabilitiesMask.MC_RESTORE_FACTORY_DEFAULTS_ENABLES_MONITOR_SETTINGS & MonitorCababilities) == MonitorCapabilitiesMask.MC_RESTORE_FACTORY_DEFAULTS_ENABLES_MONITOR_SETTINGS; }

		public Screen Screen { get => _screen; }

		public string DeviceName { get => Screen.DeviceName; }

		public BrightnessSettings BrightnessSettings => brightnessSettings;

		public ContrastSettings ContrastSettings => contrastSettings;

		internal MonitorSettings(Screen screen)
		{
			this._screen = screen;
		}
	}

	public class MonitorSettingsList : List<MonitorSettings>
	{ }
}