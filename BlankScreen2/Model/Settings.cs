using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfScreenHelper;

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

		public DisplayEntry()
		{ }

		public DisplayEntry(Screen screen)
		{
			_Enabled = false;
			_DeviceName = screen.DeviceName;
			_WpfBounds = screen.WpfBounds;
			_Bounds = screen.Bounds;
			_Primary = screen.Primary;
			_ScaleFactor = screen.ScaleFactor;
			_WorkingArea = screen.WorkingArea;
			_WpfWorkingArea = screen.WorkingArea;
		}

		public bool Enabled { get => _Enabled; set => SetField(ref _Enabled, value); }
		public string? DeviceName { get => _DeviceName; set => SetField(ref _DeviceName, value); }
		public Rect WpfBounds { get => _WpfBounds; set => _WpfBounds = value; }
		public Rect Bounds { get => _Bounds; set => SetField(ref _Bounds, value); }
		public bool Primary { get => _Primary; set => SetField(ref _Primary, value); }
		public double ScaleFactor { get => _ScaleFactor; set => SetField(ref _ScaleFactor, value); }
		public Rect WorkingArea { get => _WorkingArea; set => SetField(ref _WorkingArea, value); }
		public Rect WpfWorkingArea { get => _WpfWorkingArea; set => SetField(ref _WpfWorkingArea, value); }

		public string DisplayName
		{
			get
			{
				StringBuilder sb = new();
				if (!string.IsNullOrEmpty(DeviceName))
				{
					int pos = DeviceName.LastIndexOf('\\') + 1;
					sb.Append(DeviceName[pos..]);
				}
				sb.Append(" ");
				sb.Append(WpfBounds.Width);
				sb.Append("x");
				sb.Append(WpfBounds.Height);

				return sb.ToString();
			}
		}
	}

	public sealed class DisplayEntries : ObservableCollection<DisplayEntry>
	{
	}

	public class NotifyPropertyChanged : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return false;
			field = value;

			NotifyPropertyChanges(propertyName);
			return true;
		}

		protected void NotifyPropertyChanges([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}