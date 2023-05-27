using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BlankScreen2.Model
{
	public sealed class Settings : NotifyPropertyChanged
	{
		private DisplayEntries _DisplayEntries = new();
		private DisplayEntriesSettings _DisplayEntriesSettings = new();

		private bool _HideWindowsVolume;
		private bool _ShowDevice;
		private bool _ShowTime;
		private bool _ShowVolume;
		private Location _Location = Location.TopRight;
		private bool _BlankScreenOnStart;
		private bool _ExitOnClear;
		private bool _ShowClickScreenOnStart;
		private bool _TurnDownBrightnessContrast;
		private readonly AudioModel _AudioModel = new AudioModel();

		public bool HideWindowsVolume { get => _HideWindowsVolume; set => SetField(ref _HideWindowsVolume, value); }
		public bool ShowDevice { get => _ShowDevice; set => SetField(ref _ShowDevice, value); }
		public bool ShowTime { get => _ShowTime; set => SetField(ref _ShowTime, value); }
		public bool ShowVolume { get => _ShowVolume; set => SetField(ref _ShowVolume, value); }
		public Location Location { get => _Location; set => SetField(ref _Location, value); }
		public bool BlankScreenOnStart { get => _BlankScreenOnStart; set => SetField(ref _BlankScreenOnStart, value); }
		public bool ExitOnClear { get => _ExitOnClear; set => SetField(ref _ExitOnClear, value); }
		public bool ShowClickScreenOnStart { get => _ShowClickScreenOnStart; set => SetField(ref _ShowClickScreenOnStart, value); }
		public bool TurnDownBrightnessContrast { get => _TurnDownBrightnessContrast; set => SetField(ref _TurnDownBrightnessContrast, value); }

		[JsonIgnore]
		public DisplayEntries DisplayEntries { get => _DisplayEntries; set => SetField(ref _DisplayEntries, value); }

		[JsonIgnore]
		public AudioModel AudioModel => _AudioModel;

		public DisplayEntriesSettings DisplayEntriesSettings
		{
			get
			{
				if (_DisplayEntriesSettings == null)
					_DisplayEntriesSettings = new DisplayEntriesSettings();
				return _DisplayEntriesSettings;
			}
			set => SetField(ref _DisplayEntriesSettings, value);
		}
	}

	public sealed class DisplayEntriesSettings : List<DisplayEntrySettings>
	{
		public DisplayEntrySettings? FindByDisplayName(string deviceName)
		{
			return this.FirstOrDefault(de => de.DeviceName == deviceName);
		}
	}

	public sealed class DisplayEntrySettings
	{
		public bool Enabled { get; set; }
		public string? DeviceName { get; set; }
	}

	public enum Location
	{
		[Description("Top Left")]
		TopLeft = 0,

		[Description("Bottom Left")]
		BottomLeft = 1,

		[Description("Top Right")]
		TopRight = 2,

		[Description("Bottom Right")]
		BottomRight = 3
	};
}