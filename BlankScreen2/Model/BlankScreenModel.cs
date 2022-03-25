using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlankScreen2.ViewModel;

namespace BlankScreen2.Model
{
	public sealed class BlankScreenModel : NotifyPropertyChanged
	{
		private readonly ScreenMgr _ScreenMgr;
		private readonly int _DisplayOffset;
		private readonly AudioModel _AudioModel;
		private bool _ShowDetails;

		public bool ShowDevice { get => _ScreenMgr.Settings.ShowDevice; }
		public bool ShowTime { get => _ScreenMgr.Settings.ShowTime; }
		public bool ShowVolume { get => _ScreenMgr.Settings.ShowVolume; }
		public Location Location { get => _ScreenMgr.Settings.Location; }
		public bool ShowClickScreenOnStart { get => _ScreenMgr.Settings.ShowClickScreenOnStart; }
		public int Volume { get => _AudioModel.Volume; }
		public string DisplayName { get => DisplayEntry.DisplayName; }

		public DateTime DateTimeNow { get => DateTime.Now; }

		public DisplayEntry DisplayEntry { get => _ScreenMgr.Settings.DisplayEntries[_DisplayOffset]; }
		public bool ShowDetails { get => _ShowDetails; set => SetField(ref _ShowDetails, value); }

		public bool ShowSettings
		{ get => _ScreenMgr.ShowSettings; set { _ScreenMgr.ShowSettings = value; } }

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