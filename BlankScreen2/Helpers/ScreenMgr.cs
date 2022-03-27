﻿using System.Collections.Generic;
using System.Linq;
using BlankScreen2.Model;
using BlankScreen2.View;
using WpfScreenHelper;

namespace BlankScreen2.Helpers
{
	public sealed class ScreenMgr
	{
		private Settings _Settings;
		private AudioMgr _AudioMgr;
		private SettingsWnd? _SettingsWnd;
		private List<WndEntry> _BlankScreenWnds = new List<WndEntry>();
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
				DisplayEntry displayEntryFound = Settings.DisplayEntries.FirstOrDefault(de => de.DeviceName == screen.DeviceName);
				if (displayEntryFound == null)
				{
					DisplayEntry displayEntry = new DisplayEntry(screen);
					displayEntry.Refreshed = true;
					Settings.DisplayEntries.Add(displayEntry);
				}
				else
					displayEntryFound.Refreshed = true;
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
				WndEntry wndEntry = _BlankScreenWnds.FirstOrDefault(entry => entry.DisplayEntry == displayEntry);
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
			BlankScreenModel blankScreenModel = new BlankScreenModel(this, displayIndex, _AudioMgr.AudioModel);
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

			WndEntry wndEntry = _BlankScreenWnds.FirstOrDefault(entry => entry.BlankScreenWnd == sender);
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
			SettingPath.SaveSettings("settings", _Settings);
		}
	}
}