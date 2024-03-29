﻿using BlankScreen2.Converts;
using BlankScreen2.Model;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace BlankScreen2.View
{
	/// <summary>
	/// Interaction logic for BlankScreen.xaml
	/// </summary>
	public sealed partial class BlankScreenWnd : Window
	{
		private BlankScreenModel _BlankScreenModel;

		private DispatcherTimer? _MousePointerTimer;
		private DispatcherTimer? _ShowDetailsTimer;
		private DispatcherTimer? _ClockTickTimer;
		private bool _Closing;

		public BlankScreenWnd(BlankScreenModel blankScreenModel)
		{
			_BlankScreenModel = blankScreenModel;
			InitializeComponent();
			this.DataContext = _BlankScreenModel;
			_BlankScreenModel.PropertyChanged += _BlankScreenModel_PropertyChanged;
		}

		private void _BlankScreenModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Volume")
				ShowDeails();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			DisplayEntry displayEntry = _BlankScreenModel.DisplayEntry;
			this.Height = displayEntry.WorkingArea.Height;
			this.Width = displayEntry.WorkingArea.Width;
			this.Top = displayEntry.WorkingArea.Top;
			this.Left = displayEntry.WorkingArea.Left;

			if (_BlankScreenModel.ShowClickScreenOnStart)
				ShowDeails();

			this.WindowState = WindowState.Maximized;
			_Closing = false;

			await Task.Run(() =>
			{
				bool ret = _BlankScreenModel.DisplayEntry.Screen.GetMonitorCapabilities();
				if (ret)
					_BlankScreenModel.DisplayEntry.Screen.BackupMonitorCapabilities();
			});

			await TurnDownBrightnessContrast();
		}

		private async void Window_Closing(object sender, CancelEventArgs e)
		{
			_Closing = true;
			StopTimers();

			await _BlankScreenModel.DisplayEntry.Screen.RestoreMonitorCapabilities();
		}

		private void StopTimers()
		{
			_MousePointerTimer?.Stop();
			_ShowDetailsTimer?.Stop();
			_ClockTickTimer?.Stop();
		}

		private async Task TurnDownBrightnessContrast()
		{
			if (_BlankScreenModel.TurnDownBrightnessContrast)
			{
				await _BlankScreenModel.DisplayEntry.Screen.SetBrightness(0);
				await _BlankScreenModel.DisplayEntry.Screen.SetContrast(0);
			}
		}

		private async void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			StopTimers();

			ContextMenu cm = new ContextMenu();
			cm.Items.Add(CreateMenu("Show Settings", MenuItem_ShowSettingsClick));

			cm.Items.Add(new Separator());
			foreach (DisplayEntry displayEntry in _BlankScreenModel.DisplayEntries)
			{
				string displayName = DisplayNameConverter.ConvertDisplayName(displayEntry.DeviceName, displayEntry.WpfWorkingArea);
				cm.Items.Add(CreateMenu(displayName, MenuItem_ShowHideScreen, displayEntry.Enabled));
			}

			cm.Items.Add(new Separator());
			cm.Items.Add(CreateMenu("Exit", MenuItem_ExitClick));
			cm.Closed += Cm_Closed;
			cm.IsOpen = true;

			if (_BlankScreenModel.DisplayEntry.Screen.NeedsRestore())
				await _BlankScreenModel.DisplayEntry.Screen.RestoreMonitorCapabilities();
		}

		private void Cm_Closed(object sender, RoutedEventArgs e)
		{
			if (!_Closing)
				StartTimer(ref _MousePointerTimer, 5, MousePointerTimer_Tick);
		}

		private static MenuItem CreateMenu(string strHeader, RoutedEventHandler reh, bool bIsChecked = false)
		{
			MenuItem mi = new MenuItem
			{
				Header = strHeader
			};
			if (reh != null)
				mi.Click += reh;

			mi.IsChecked = bIsChecked;

			return mi;
		}

		private void MenuItem_ExitClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void MenuItem_ShowSettingsClick(object sender, RoutedEventArgs e)
		{
			_BlankScreenModel.ShowSettings = true;
			this.Close();
		}

		private void MenuItem_ShowHideScreen(object sender, RoutedEventArgs e)
		{
			if (!(sender is MenuItem menuItem))
				return;

			if (!(menuItem.Header is string header))
				return;

			DisplayEntry? displayEntry = _BlankScreenModel.DisplayEntries.FindByDisplayName(header);
			if (displayEntry != null)
				_BlankScreenModel.ScreenMgr.ShowHideBlankScreen(displayEntry);

			return;
		}

		private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ShowDeails();
		}

		private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.Close();
		}

		private async void Window_MouseMove(object sender, MouseEventArgs e)
		{
			Point mousePos = e.GetPosition(this);
			if (MouseMoved(mousePos) == true)
			{
				StartTimer(ref _MousePointerTimer, 5, MousePointerTimer_Tick);

				ShowMouse(true);
				if (_BlankScreenModel.DisplayEntry.Screen.NeedsRestore())
					await _BlankScreenModel.DisplayEntry.Screen.RestoreMonitorCapabilities();
			}
		}

		private static void StartTimer(ref DispatcherTimer? dispatcherTimer, int intervalSec, EventHandler eventHandler)
		{
			dispatcherTimer?.Stop();
			dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Interval = TimeSpan.FromSeconds(intervalSec);
			dispatcherTimer.Tick += eventHandler;
			dispatcherTimer.Start();
		}

		private void ClockTickTimer_Tick(object? sender, EventArgs e)
		{
			_BlankScreenModel.Tick();
		}

		private void ShowDetailsTimer_Tick(object? sender, EventArgs e)
		{
			if (_ShowDetailsTimer == null)
				return;

			_ShowDetailsTimer.Stop();
			_ShowDetailsTimer = null;

			_ClockTickTimer?.Stop();
			_ClockTickTimer = null;

			_BlankScreenModel.ShowDetails = false;
		}

		private async void MousePointerTimer_Tick(object? sender, EventArgs e)
		{
			if (!(sender is DispatcherTimer dispatcherTimer))
				return;

			dispatcherTimer.Stop();
			ShowMouse(false);
			await TurnDownBrightnessContrast();
		}

		private bool MouseMoved(Point currentPos)
		{
			if (!_BlankScreenModel.MouseLastPos.HasValue)
			{
				_BlankScreenModel.MouseLastPos = currentPos;
				return true;
			}

			double difX = Math.Abs(currentPos.X - _BlankScreenModel.MouseLastPos.Value.X);
			double difY = Math.Abs(currentPos.Y - _BlankScreenModel.MouseLastPos.Value.Y);

			_BlankScreenModel.MouseLastPos = currentPos;
			double diffVal = 2;

			if ((difX > diffVal) || (difY > diffVal))
				return true;

			return false;
		}

		private void ShowMouse(bool show)
		{
			if (show)
				this.Cursor = null;
			else
				this.Cursor = Cursors.None;
		}

		private void ShowDeails()
		{
			if (Thread.CurrentThread != Dispatcher.Thread)
			{
				this.Dispatcher.Invoke(new Action(() => ShowDeails()));
				return;
			}

			_BlankScreenModel.ShowDetails = true;
			_BlankScreenModel.Tick();

			StartTimer(ref _ShowDetailsTimer, 5, ShowDetailsTimer_Tick);
			StartTimer(ref _ClockTickTimer, 1, ClockTickTimer_Tick);
		}
	}
}