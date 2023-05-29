using HVWpfScreenHelper;
using System.Text.Json.Serialization;
using System.Windows;

namespace BlankScreen2.Model
{
	public sealed class DisplayEntry : NotifyPropertyChanged
	{
		private Screen _Screen;
		private bool _Enabled = false;

		private bool _Refreshed = false;

		public DisplayEntry(Screen screen)
		{
			_Screen = screen;
		}

		public bool Enabled { get => _Enabled; set => SetField(ref _Enabled, value); }
		public string? DeviceName => Screen?.DeviceName;

		[JsonIgnore]
		public Rect WpfBounds => Screen.WpfBounds;

		[JsonIgnore]
		public Rect Bounds => Screen.Bounds;

		[JsonIgnore]
		public bool Primary => Screen.Primary;

		[JsonIgnore]
		public double ScaleFactor => Screen.ScaleFactor;

		[JsonIgnore]
		public Rect WorkingArea => Screen.WorkingArea;

		[JsonIgnore]
		public Rect WpfWorkingArea => Screen.WpfWorkingArea;

		[JsonIgnore]
		public bool Refreshed { get => _Refreshed; set => _Refreshed = value; }

		[JsonIgnore]
		public Screen Screen { get => _Screen; set => _Screen = value; }
	}
}