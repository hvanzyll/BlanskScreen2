using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using BlankScreen2.Model;
using BlankScreen2.View;
using WpfScreenHelper;

namespace BlankScreen2.ViewModel
{
	public sealed class ScreenMgr
	{
		private Settings _Settings;
		private AudioMgr _AudioMgr;
		private SettingsWnd? _SettingsWnd;
		private List<BlankScreenWnd> _BlankScreenWnds = new List<BlankScreenWnd>();
		public bool ShowSettings { get; set; }

		public Settings Settings { get => _Settings; set => _Settings = value; }

		public ScreenMgr()
		{
			_Settings = SettingPath.LoadSettings<Settings>("settings");

			AudioModel audioModel = new();
			_AudioMgr = new(audioModel);
		}

		public void RefreshDisplays()
		{
			List<Screen> screens = Screen.AllScreens.ToList();

			foreach (Screen screen in screens)
			{
				bool t = Settings.DisplayEntries.Any(de => de.DeviceName == screen.DeviceName);
				if (!t)
					Settings.DisplayEntries.Add(new DisplayEntry(screen));
			}
		}

		public void ShowWindow()
		{
			if (_Settings.BlankScreenOnStart)
			{
				ShowBlankScreen();
				return;
			}

			ShowSettingsWnd();
		}

		public void ShowSettingsWnd()
		{
			_SettingsWnd = new(this);
			_SettingsWnd.Closing += _SettingsWnd_Closing;
			_SettingsWnd.Show();
		}

		private void _SettingsWnd_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
		{
			if (_SettingsWnd != null)
				_SettingsWnd.Closing -= _SettingsWnd_Closing;

			SaveSettings();
		}

		public void ShowBlankScreen()
		{
			ShowSettings = false;

			for (int displayIndex = 0; displayIndex < _Settings.DisplayEntries.Count; displayIndex++)
			{
				DisplayEntry displayEntry = Settings.DisplayEntries[displayIndex];
				if (displayEntry.Enabled)
				{
					BlankScreenModel blankScreenModel = new BlankScreenModel(this, displayIndex, _AudioMgr.AudioModel);
					BlankScreenWnd blankScreenWnd = new BlankScreenWnd(blankScreenModel);
					_BlankScreenWnds.Add(blankScreenWnd);
					blankScreenWnd.Closing += BlankScreenWnd_Closing;
					blankScreenWnd.Show();
				}
			}
		}

		private void BlankScreenWnd_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!_Settings.ExitOnClear || ShowSettings)
				ShowSettingsWnd();

			if (sender is BlankScreenWnd blankScreenWnd)
				_BlankScreenWnds.Remove(blankScreenWnd);

			for (int screenIndex = 0; screenIndex < _BlankScreenWnds.Count; screenIndex++)
			{
				BlankScreenWnd wnd = _BlankScreenWnds[screenIndex];
				wnd.Close();
			}
			_BlankScreenWnds.Clear();
		}

		private void SaveSettings()
		{
			SettingPath.SaveSettings("settings", _Settings);
		}
	}
}