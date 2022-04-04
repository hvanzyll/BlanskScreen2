using System.Text;
using System.Windows;
using WpfScreenHelper;

namespace BlankScreen2.Model
{
	public sealed class DisplayEntry : NotifyPropertyChanged
	{
		private bool _Enabled;
		private string? _DeviceName;
		private Rect _WpfBounds;
		private Rect _Bounds;
		private bool _Primary;
		private double _ScaleFactor = 1.0;
		private Rect _WorkingArea;
		private Rect _WpfWorkingArea;

		private bool _Refreshed;

		public DisplayEntry()
		{ }

		public DisplayEntry(Screen screen)
		{
			_Enabled = false;
			SetScreenData(screen);
		}

		public void SetScreenData(Screen screen)
		{
			DeviceName = screen.DeviceName;
			WpfBounds = screen.WpfBounds;
			Bounds = screen.Bounds;
			Primary = screen.Primary;
			ScaleFactor = screen.ScaleFactor;
			WorkingArea = screen.WorkingArea;
			WpfWorkingArea = screen.WorkingArea;
		}

		public bool Enabled { get => _Enabled; set => SetField(ref _Enabled, value); }
		public string? DeviceName { get => _DeviceName; set => SetField(ref _DeviceName, value); }
		public Rect WpfBounds { get => _WpfBounds; set => _WpfBounds = value; }
		public Rect Bounds { get => _Bounds; set => SetField(ref _Bounds, value); }
		public bool Primary { get => _Primary; set => SetField(ref _Primary, value); }
		public double ScaleFactor { get => _ScaleFactor; set => SetField(ref _ScaleFactor, value); }
		public Rect WorkingArea { get => _WorkingArea; set => SetField(ref _WorkingArea, value); }
		public Rect WpfWorkingArea { get => _WpfWorkingArea; set => SetField(ref _WpfWorkingArea, value); }

		public bool Refreshed { get => _Refreshed; set => _Refreshed = value; }
	}
}