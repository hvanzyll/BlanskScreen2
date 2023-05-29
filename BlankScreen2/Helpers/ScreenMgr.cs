using BlankScreen2.Model;
using BlankScreen2.View;
using HVWpfScreenHelper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlankScreen2.Helpers
{
	public sealed class ScreenMgr
	{
		private Settings _Settings;
		private AudioMgr _AudioMgr;
		private SettingsWnd? _SettingsWnd;
		private readonly List<WndEntry> _BlankScreenWnds = new List<WndEntry>();
		public bool ShowSettings { get; set; }

		public Settings Settings { get => _Settings; set => _Settings = value; }

		public ScreenMgr()
		{
			_Settings = LoadSettings();
			RefreshDisplays();

			_AudioMgr = new AudioMgr();
			_AudioMgr.VolumeUpdatedEvent += AudioMgr_VolumeUpdatedEvent;
		}

		private static Settings LoadSettings()
		{
			Settings? settings = SettingPath.LoadSettings<Settings>("settings");
			if (settings == null)
				settings = new Settings();
			return settings;
		}

		private void AudioMgr_VolumeUpdatedEvent(object? sender, VolumeUpdatedEventArgs e)
		{
			_Settings.AudioModel.Volume = e.Volume;
			_Settings.AudioModel.DeviceName = e.DeviceName;
		}

		public void RefreshDisplays()
		{
			List<Screen> screens = Screen.AllScreens.ToList();

			Settings.DisplayEntries.ResetRefreshed();

			foreach (Screen screen in screens)
			{
				DisplayEntry? displayEntryFound = Settings.DisplayEntries.FindByDisplayName(screen.DeviceName);
				if (displayEntryFound == null)
				{
					DisplayEntrySettings? displayEntrySettings = Settings.DisplayEntriesSettings.FindByDisplayName(screen.DeviceName);

					DisplayEntry displayEntry = new DisplayEntry(screen);
					displayEntry.Refreshed = true;

					if (displayEntrySettings != null && !string.IsNullOrEmpty(displayEntrySettings.DeviceName))
						displayEntry.Enabled = displayEntrySettings.Enabled;

					Settings.DisplayEntries.Add(displayEntry);
				}
				else
				{
					displayEntryFound.Screen = screen;
					displayEntryFound.Refreshed = true;
				}
			}

			Settings.DisplayEntries.ClearAllNonRefreshed();
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

		public async Task ResetBrightnessContrast()
		{
			for (int displayIndex = 0; displayIndex < _Settings.DisplayEntries.Count; displayIndex++)
			{
				DisplayEntry displayEntry = Settings.DisplayEntries[displayIndex];
				await displayEntry.Screen.SetBrightness(75);
				await displayEntry.Screen.SetContrast(75);
			}
		}

		public void ShowSettingsWnd()
		{
			_SettingsWnd = new(this);
			_SettingsWnd.Closing += SettingsWnd_Closing;
			_SettingsWnd.Show();
		}

		private void SettingsWnd_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
		{
			if (_SettingsWnd != null)
				_SettingsWnd.Closing -= SettingsWnd_Closing;

			SaveSettings();
		}

		public void ShowBlankScreen()
		{
			ShowSettings = false;

			if (_Settings.HideWindowsVolume)
				_AudioMgr.HideWindowsVolume(true);

			for (int displayIndex = 0; displayIndex < _Settings.DisplayEntries.Count; displayIndex++)
			{
				DisplayEntry displayEntry = Settings.DisplayEntries[displayIndex];
				if (displayEntry.Enabled)
					ShowBlackScreen(displayIndex);
			}
		}

		public void ShowHideBlankScreen(DisplayEntry displayEntry)
		{
			if (displayEntry.Enabled)
			{
				WndEntry? wndEntry = _BlankScreenWnds.FirstOrDefault(entry => entry.DisplayEntry == displayEntry);
				if (wndEntry == null)
					return;

				// check if last entry, if so close and go back to settings.
				if (_BlankScreenWnds.Count == 1)
				{
					ShowSettings = true;
					wndEntry.BlankScreenWnd.Close();
					return;
				}

				displayEntry.Enabled = false;
				wndEntry.BlankScreenWnd.Closing -= BlankScreenWnd_Closing;
				wndEntry.BlankScreenWnd.Close();
				_BlankScreenWnds.Remove(wndEntry);

				return;
			}

			displayEntry.Enabled = true;
			int displayIndex = _Settings.DisplayEntries.GetDisplayIndex(displayEntry);
			if (displayIndex < 0)
				return;

			ShowBlackScreen(displayIndex);
		}

		private void ShowBlackScreen(int displayIndex)
		{
			BlankScreenModel blankScreenModel = new BlankScreenModel(this, displayIndex, _Settings.AudioModel);
			BlankScreenWnd blankScreenWnd = new BlankScreenWnd(blankScreenModel);
			_BlankScreenWnds.Add(new WndEntry(blankScreenWnd, blankScreenModel.DisplayEntry));
			blankScreenWnd.Closing += BlankScreenWnd_Closing;
			blankScreenWnd.Show();
		}

		private void BlankScreenWnd_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!_Settings.ExitOnClear || ShowSettings)
				ShowSettingsWnd();

			if (_Settings.HideWindowsVolume)
				_AudioMgr.HideWindowsVolume(false);

			WndEntry? wndEntry = _BlankScreenWnds.FirstOrDefault(entry => entry.BlankScreenWnd == sender);
			if (wndEntry != null)
				_BlankScreenWnds.Remove(wndEntry);

			for (int screenIndex = 0; screenIndex < _BlankScreenWnds.Count; screenIndex++)
			{
				BlankScreenWnd wnd = _BlankScreenWnds[screenIndex].BlankScreenWnd;
				wnd.Close();
			}
			_BlankScreenWnds.Clear();
		}

		private void SaveSettings()
		{
			_Settings.DisplayEntriesSettings.Clear();

			foreach (DisplayEntry displayEntry in _Settings.DisplayEntries)
				_Settings.DisplayEntriesSettings.Add(new DisplayEntrySettings()
				{
					DeviceName = displayEntry.DeviceName,
					Enabled = displayEntry.Enabled
				});

			SettingPath.SaveSettings("settings", _Settings);
		}
	}
}