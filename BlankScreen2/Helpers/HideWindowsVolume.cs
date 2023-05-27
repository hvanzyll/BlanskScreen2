using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BlankScreen2.Helpers
{
	public sealed class HideWindowsVolume
	{
		private IntPtr hWndInject = IntPtr.Zero;

		[DllImport("user32.dll")]
		private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

		private const int VOLUME_DOWN_KEY = 0xAE;
		private const int VOLUME_UP_KEY = 0xAF;

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr FindWindowEx(
		  IntPtr hwndParent,
		  IntPtr hwndChildAfter,
		  string lpszClass,
		  string lpszWindow);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		private const int SW_MINIMIZE = 6;
		private const int SW_RESTORE = 9;

		[DllImport("user32.dll")]
		private static extern bool IsWindow(IntPtr hWnd);

		private void Init()
		{
			int seconds = 5;
			int sleep = 10;
			int retries = seconds * 1000 / sleep;
			for (int count = 1; this.hWndInject == IntPtr.Zero && count < retries; ++count)
			{
				keybd_event((byte)VOLUME_DOWN_KEY, (byte)0, 0U, 0);
				keybd_event((byte)VOLUME_UP_KEY, (byte)0, 0U, 0);
				this.hWndInject = FindOSDWindow(true);
				Thread.Sleep(sleep);
			}
		}

		private IntPtr FindOSDWindow(bool bSilent)
		{
			IntPtr osdWindow = IntPtr.Zero;
			IntPtr NativeHWNDHostWnd = IntPtr.Zero;
			int numWindowsFound = 0;
			while ((NativeHWNDHostWnd = FindWindowEx(IntPtr.Zero, NativeHWNDHostWnd, "NativeHWNDHost", "")) != IntPtr.Zero)
			{
				if (FindWindowEx(NativeHWNDHostWnd, IntPtr.Zero, "DirectUIHWND", "") != IntPtr.Zero)
				{
					if (numWindowsFound == 0)
						osdWindow = NativeHWNDHostWnd;
					++numWindowsFound;
					if (numWindowsFound > 1)
					{
						MessageBox.Show("Severe error: Multiple pairs found!", "HideVolumeOSD");
						return IntPtr.Zero;
					}
				}
			}
			if (osdWindow == IntPtr.Zero && !bSilent)
			{
				MessageBox.Show("Severe error: OSD window not found!", "HideVolumeOSD");
			}
			return osdWindow;
		}

		public void HideOSD()
		{
			Task.Run(() =>
			{
				if (!IsWindow(this.hWndInject))
					this.Init();
				ShowWindow(this.hWndInject, SW_MINIMIZE);
			});
		}

		public void ShowOSD()
		{
			Task.Run(() =>
			{
				if (!IsWindow(this.hWndInject))
					this.Init();
				ShowWindow(this.hWndInject, SW_RESTORE);
			});
		}
	}
}