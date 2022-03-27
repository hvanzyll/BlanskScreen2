using System.ComponentModel;

namespace BlankScreen2.Model
{
	public sealed class Settings : NotifyPropertyChanged
	{
		private DisplayEntries _DisplayEntries = new();
		private bool _HideWindowsVolume;
		private bool _ShowDevice;
		private bool _ShowTime;
		private bool _ShowVolume;
		private Location _Location = Location.TopRight;
		private bool _BlankScreenOnStart;
		private bool _ExitOnClear;
		private bool _ShowClickScreenOnStart;

		public bool HideWindowsVolume { get => _HideWindowsVolume; set => SetField(ref _HideWindowsVolume, value); }
		public bool ShowDevice { get => _ShowDevice; set => SetField(ref _ShowDevice, value); }
		public bool ShowTime { get => _ShowTime; set => SetField(ref _ShowTime, value); }
		public bool ShowVolume { get => _ShowVolume; set => SetField(ref _ShowVolume, value); }
		public Location Location { get => _Location; set => SetField(ref _Location, value); }
		public bool BlankScreenOnStart { get => _BlankScreenOnStart; set => SetField(ref _BlankScreenOnStart, value); }
		public bool ExitOnClear { get => _ExitOnClear; set => SetField(ref _ExitOnClear, value); }
		public bool ShowClickScreenOnStart { get => _ShowClickScreenOnStart; set => SetField(ref _ShowClickScreenOnStart, value); }
		public DisplayEntries DisplayEntries { get => _DisplayEntries; set => SetField(ref _DisplayEntries, value); }
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