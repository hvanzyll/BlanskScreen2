using BlankScreen2.Helpers;
using System;

namespace BlankScreen2.Model
{
	public sealed class BlankScreenModel : NotifyPropertyChanged
	{
		private readonly ScreenMgr _ScreenMgr;
		private readonly int _DisplayOffset;
		private readonly AudioModel _AudioModel;
		private bool _ShowDetails;

		public bool ShowDevice { get => ScreenMgr.Settings.ShowDevice; }
		public bool ShowTime { get => ScreenMgr.Settings.ShowTime; }
		public bool ShowVolume { get => ScreenMgr.Settings.ShowVolume; }
		public Location Location { get => ScreenMgr.Settings.Location; }
		public bool ShowClickScreenOnStart { get => ScreenMgr.Settings.ShowClickScreenOnStart; }
		public int Volume { get => _AudioModel.Volume; }
		public string? DeviceName { get => _AudioModel.DeviceName; }
		public string DisplayName { get => DisplayEntry.DisplayName; }

		public DateTime DateTimeNow { get => DateTime.Now; }

		public DisplayEntry DisplayEntry { get => ScreenMgr.Settings.DisplayEntries[_DisplayOffset]; }

		public DisplayEntries DisplayEntries { get => ScreenMgr.Settings.DisplayEntries; }
		public bool ShowDetails { get => _ShowDetails; set => SetField(ref _ShowDetails, value); }

		public bool ShowSettings
		{ get => ScreenMgr.ShowSettings; set { ScreenMgr.ShowSettings = value; } }

		public ScreenMgr ScreenMgr => _ScreenMgr;

		public BlankScreenModel(ScreenMgr screenMgr, int displayOffset, AudioModel audioModel)
		{
			_ScreenMgr = screenMgr;
			_DisplayOffset = displayOffset;
			_AudioModel = audioModel;

			_ScreenMgr.Settings.PropertyChanged += Dependants_PropertyChanged;
			_AudioModel.PropertyChanged += Dependants_PropertyChanged;
		}

		public void Tick()
		{
			NotifyPropertyChanges("DateTimeNow");
		}

		private void Dependants_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e is null)
				return;
			if (string.IsNullOrEmpty(e.PropertyName))
				return;

			NotifyPropertyChanges(e.PropertyName);
		}
	}
}