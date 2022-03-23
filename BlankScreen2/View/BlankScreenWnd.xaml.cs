using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlankScreen2.Model;
using BlankScreen2.ViewModel;

namespace BlankScreen2.View
{
	/// <summary>
	/// Interaction logic for BlankScreen.xaml
	/// </summary>
	public sealed partial class BlankScreenWnd : Window
	{
		private ScreenMgr _ScreenMgr;
		private int _DisplayOffset;
		private DispatcherTimer? _Timer;
		private Point? _MouseLastPos;

		public BlankScreenWnd(ScreenMgr screenMgr, int displayOffset)
		{
			_ScreenMgr = screenMgr;
			_DisplayOffset = displayOffset;
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			DisplayEntry displayEntry = GetDisplayEntry();
			this.Height = displayEntry.WpfBounds.Height;
			this.Width = displayEntry.WpfBounds.Width;
			this.Top = displayEntry.WpfBounds.Top;
			this.Left = displayEntry.WpfBounds.Left;
		}

		private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			ContextMenu cm = new ContextMenu();
			cm.Items.Add(CreateMenu("Show Settings", MenuItem_ShowSettingsClick));
			cm.Items.Add(CreateMenu("Exit", MenuItem_ExitClick));
			cm.IsOpen = true;
		}

		private MenuItem CreateMenu(string strHeader, RoutedEventHandler REH = null, bool bIsChecked = false)
		{
			MenuItem mi = new MenuItem
			{
				Header = strHeader
			};
			if (REH != null)
				mi.Click += REH;

			mi.IsChecked = bIsChecked;

			return mi;
		}

		private void MenuItem_ExitClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void MenuItem_ShowSettingsClick(object sender, RoutedEventArgs e)
		{
			_ScreenMgr.Settings.ShowSettings = true;
			this.Close();
		}

		private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.Close();
		}

		private DisplayEntry GetDisplayEntry()
		{
			return _ScreenMgr.Settings.DisplayEntries[_DisplayOffset];
		}

		private void Window_MouseMove(object sender, MouseEventArgs e)
		{
			Point mousePos = e.GetPosition(this);
			if (MouseMoved(mousePos) == true)
			{
				if (_Timer != null)
					_Timer.Stop();
				_Timer = new DispatcherTimer();
				_Timer.Interval = TimeSpan.FromSeconds(5);
				_Timer.Tick += _Timer_Tick; ;
				_Timer.Start();

				ShowMouse(true);
			}
		}

		private void _Timer_Tick(object? sender, EventArgs e)
		{
			if (!(sender is DispatcherTimer dispatcherTimer))
				return;

			dispatcherTimer.Stop();
			ShowMouse(false);
		}

		private bool MouseMoved(Point currentPos)
		{
			if (!_MouseLastPos.HasValue)
			{
				_MouseLastPos = currentPos;
				return true;
			}

			double difX = Math.Abs(currentPos.X - _MouseLastPos.Value.X);
			double difY = Math.Abs(currentPos.Y - _MouseLastPos.Value.Y);

			_MouseLastPos = currentPos;
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
	}
}