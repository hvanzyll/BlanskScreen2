using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using static HVWpfScreenHelper.NativeMethods;

namespace HVWpfScreenHelper
{
	public class Screen
	{
		private class MonitorEnumCallback
		{
			public ArrayList Screens { get; }

			public MonitorEnumCallback()
			{
				Screens = new ArrayList();
			}

			public bool Callback(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lparam)
			{
				Screens.Add(new Screen(monitor, hdc));
				return true;
			}
		}

		private static readonly bool MultiMonitorSupport;

		private const int PRIMARY_MONITOR = -1163005939;

		private const int MONITORINFOF_PRIMARY = 1;

		private readonly IntPtr monitorHandle;
		private IntPtr hPhysicalMonitor;

		private NativeMethods.MonitorCapabilitiesMask monitorCababilities = MonitorCapabilitiesMask.MC_CAPS_NONE;
		private NativeMethods.ColorTemperatureMask colorTemperature = ColorTemperatureMask.MC_SUPPORTED_COLOR_TEMPERATURE_NONE;

		public readonly BrightnessSettings brightnessSettings = new BrightnessSettings();
		public readonly ContrastSettings contrastSettings = new ContrastSettings();

		public static IEnumerable<Screen> AllScreens
		{
			get
			{
				if (MultiMonitorSupport)
				{
					MonitorEnumCallback monitorEnumCallback = new MonitorEnumCallback();
					NativeMethods.MonitorEnumProc lpfnEnum = monitorEnumCallback.Callback;
					NativeMethods.EnumDisplayMonitors(NativeMethods.NullHandleRef, null, lpfnEnum, IntPtr.Zero);
					if (monitorEnumCallback.Screens.Count > 0)
					{
						return monitorEnumCallback.Screens.Cast<Screen>();
					}
				}

				return new Screen[1]
				{
					new Screen((IntPtr)(-1163005939))
				};
			}
		}

		public static Screen PrimaryScreen
		{
			get
			{
				if (!MultiMonitorSupport)
				{
					return new Screen((IntPtr)(-1163005939));
				}

				return AllScreens.FirstOrDefault((Screen t) => t.Primary);
			}
		}

		public Rect WpfBounds
		{
			get
			{
				if (!ScaleFactor.Equals(1.0))
				{
					return new Rect(Bounds.X / ScaleFactor, Bounds.Y / ScaleFactor, Bounds.Width / ScaleFactor, Bounds.Height / ScaleFactor);
				}

				return Bounds;
			}
		}

		public string DeviceName { get; }

		public Rect Bounds { get; }

		public bool Primary { get; }

		public double ScaleFactor { get; } = 1.0;

		public Rect WorkingArea
		{
			get
			{
				Rect result;
				if (!MultiMonitorSupport || monitorHandle == (IntPtr)(-1163005939))
				{
					NativeMethods.RECT rc = default(NativeMethods.RECT);
					NativeMethods.SystemParametersInfo(NativeMethods.SPI.SPI_GETWORKAREA, 0, ref rc, NativeMethods.SPIF.SPIF_SENDCHANGE);
					result = new Rect(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
				}
				else
				{
					NativeMethods.MONITORINFOEX mONITORINFOEX = new NativeMethods.MONITORINFOEX();
					NativeMethods.GetMonitorInfo(new HandleRef(null, monitorHandle), mONITORINFOEX);
					result = new Rect(mONITORINFOEX.rcWork.left, mONITORINFOEX.rcWork.top, mONITORINFOEX.rcWork.right - mONITORINFOEX.rcWork.left, mONITORINFOEX.rcWork.bottom - mONITORINFOEX.rcWork.top);
				}

				return result;
			}
		}

		public Rect WpfWorkingArea
		{
			get
			{
				if (!ScaleFactor.Equals(1.0))
				{
					return new Rect(WorkingArea.X / ScaleFactor, WorkingArea.Y / ScaleFactor, WorkingArea.Width / ScaleFactor, WorkingArea.Height / ScaleFactor);
				}

				return WorkingArea;
			}
		}

		protected IntPtr MonitorHandle
		{ get { return monitorHandle; } }

		static Screen()
		{
			MultiMonitorSupport = NativeMethods.GetSystemMetrics(NativeMethods.SystemMetric.SM_CMONITORS) != 0;
		}

		private Screen(IntPtr monitor)
			: this(monitor, IntPtr.Zero)
		{
		}

		private Screen(IntPtr monitor, IntPtr hdc)
		{
			if (NativeMethods.IsProcessDPIAware())
			{
				uint dpiX;
				try
				{
					uint dpiY;
					if (monitor == (IntPtr)(-1163005939))
					{
						NativeMethods.GetDpiForMonitor(NativeMethods.MonitorFromPoint(new NativeMethods.POINTSTRUCT(0, 0), NativeMethods.MonitorDefault.MONITOR_DEFAULTTOPRIMARY), NativeMethods.DpiType.EFFECTIVE, out dpiX, out dpiY);
					}
					else
					{
						NativeMethods.GetDpiForMonitor(monitor, NativeMethods.DpiType.EFFECTIVE, out dpiX, out dpiY);
					}
				}
				catch
				{
					if (NativeMethods.D2D1CreateFactory(NativeMethods.D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, typeof(NativeMethods.ID2D1Factory).GUID, IntPtr.Zero, out var ppIFactory) < 0)
					{
						dpiX = 96u;
					}
					else
					{
						ppIFactory.GetDesktopDpi(out var dpiX2, out var _);
						Marshal.ReleaseComObject(ppIFactory);
						dpiX = (uint)dpiX2;
					}
				}

				ScaleFactor = (double)dpiX / 96.0;
			}

			if (!MultiMonitorSupport || monitor == (IntPtr)(-1163005939))
			{
				Size size = new Size(NativeMethods.GetSystemMetrics(NativeMethods.SystemMetric.SM_CXSCREEN), NativeMethods.GetSystemMetrics(NativeMethods.SystemMetric.SM_CYSCREEN));
				Bounds = new Rect(0.0, 0.0, size.Width, size.Height);
				Primary = true;
				DeviceName = "DISPLAY";
			}
			else
			{
				NativeMethods.MONITORINFOEX mONITORINFOEX = new NativeMethods.MONITORINFOEX();
				NativeMethods.GetMonitorInfo(new HandleRef(null, monitor), mONITORINFOEX);
				Bounds = new Rect(mONITORINFOEX.rcMonitor.left, mONITORINFOEX.rcMonitor.top, mONITORINFOEX.rcMonitor.right - mONITORINFOEX.rcMonitor.left, mONITORINFOEX.rcMonitor.bottom - mONITORINFOEX.rcMonitor.top);
				Primary = (mONITORINFOEX.dwFlags & 1) != 0;
				DeviceName = new string(mONITORINFOEX.szDevice).TrimEnd('\0');
			}

			monitorHandle = monitor;
		}

		public static Screen FromHandle(IntPtr hwnd)
		{
			if (!MultiMonitorSupport)
			{
				return new Screen((IntPtr)(-1163005939));
			}

			return new Screen(NativeMethods.MonitorFromWindow(new HandleRef(null, hwnd), 2));
		}

		public static Screen FromPoint(Point point)
		{
			if (MultiMonitorSupport)
			{
				return new Screen(NativeMethods.MonitorFromPoint(new NativeMethods.POINTSTRUCT((int)point.X, (int)point.Y), NativeMethods.MonitorDefault.MONITOR_DEFAULTTONEAREST));
			}

			return new Screen((IntPtr)(-1163005939));
		}

		public static Screen FromWindow(Window window)
		{
			return FromHandle(new WindowInteropHelper(window).Handle);
		}

		public override bool Equals(object? obj)
		{
			Screen? screen = obj as Screen;
			if (screen != null && monitorHandle == screen.monitorHandle)
			{
				return true;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return monitorHandle.GetHashCode();
		}

		public bool GetMonitorCapabilities()
		{
			int monitorCount = 0;
			bool ret = NativeMethods.GetNumberOfPhysicalMonitorsFromHMONITOR(monitorHandle, ref monitorCount);
			if (!ret || monitorCount != 1)
				return false;

			PHYSICAL_MONITOR pHYSICAL_MONITOR = new PHYSICAL_MONITOR
			{
				szPhysicalMonitorDescription = new System.Char[NativeMethods.PHYSICAL_MONITOR_DESCRIPTION_SIZE]
			};

			NativeMethods.GetPhysicalMonitorsFromHMONITOR(MonitorHandle, monitorCount, ref pHYSICAL_MONITOR);

			hPhysicalMonitor = pHYSICAL_MONITOR.hPhysicalMonitor;

			ret = NativeMethods.GetMonitorCapabilities(hPhysicalMonitor, ref monitorCababilities, ref colorTemperature);

			return ret;
		}

		public bool HasMonitorCapabilitiesBrightness()
		{
			return (monitorCababilities & NativeMethods.MonitorCapabilitiesMask.MC_CAPS_BRIGHTNESS) == NativeMethods.MonitorCapabilitiesMask.MC_CAPS_BRIGHTNESS;
		}

		public bool HasMonitorCapabilitiesContrast()
		{
			return (monitorCababilities & NativeMethods.MonitorCapabilitiesMask.MC_CAPS_CONTRAST) == MonitorCapabilitiesMask.MC_CAPS_CONTRAST;
		}

		public void BackupMonitorCapabilities()
		{
			if (HasMonitorCapabilitiesBrightness())
				BackupBrightness();

			if (HasMonitorCapabilitiesContrast())
				BackupContrast();
		}

		public async Task RestoreMonitorCapabilities()
		{
			if (HasMonitorCapabilitiesBrightness())
				await RestoreBrightness();

			if (HasMonitorCapabilitiesContrast())
				await RestoreContrast();
		}

		#region Brightness

		private bool BackupBrightness()
		{
			int pdwMinimumBrightness = 0;
			int pdwCurrentBrightness = 0;
			int pdwMaximumBrightness = 0;
			bool ret = NativeMethods.GetMonitorBrightness(hPhysicalMonitor, ref pdwMinimumBrightness, ref pdwCurrentBrightness, ref pdwMaximumBrightness);
			if (ret)
			{
				brightnessSettings.MinimumBrightness = pdwMinimumBrightness;
				brightnessSettings.CurrentBrightness = pdwCurrentBrightness;
				brightnessSettings.MaximumBrightness = pdwMaximumBrightness;
				brightnessSettings.SettingsSet();
			}

			return ret;
		}

		private async Task<bool> RestoreBrightness()
		{
			if (!brightnessSettings.AreSettingsSet())
				return false;

			bool ret = await SetBrightness(brightnessSettings.CurrentBrightness);

			return ret;
		}

		public async Task<bool> SetBrightness(int brightness)
		{
			if (!HasMonitorCapabilitiesBrightness())
				return false;

			int oldBrightness = GetBrightness();

			brightness = Math.Max(Math.Min(brightness, brightnessSettings.MaximumBrightness), brightnessSettings.MinimumBrightness);
			bool ret = NativeMethods.SetMonitorBrightness(hPhysicalMonitor, brightness);
			if (!ret)
				return false;

			int newBrightness = GetBrightness();
			int counter = 0;
			while (newBrightness == -1 || counter > 10)
			{
				await Task.Delay(10);
				newBrightness = GetBrightness();
				counter++;
			};

			return newBrightness != -1;
		}

		public int GetBrightness()
		{
			if (!HasMonitorCapabilitiesBrightness())
				return -1;

			int pdwMinimumBrightness = 0;
			int pdwCurrentBrightness = 0;
			int pdwMaximumBrightness = 0;
			bool ret = GetBrightness(ref pdwMinimumBrightness, ref pdwCurrentBrightness, ref pdwMaximumBrightness);
			if (ret)
				return pdwCurrentBrightness;

			return -1;
		}

		public bool GetBrightness(ref int pdwMinimumBrightness, ref int pdwCurrentBrightness, ref int pdwMaximumBrightness)
		{
			bool ret = NativeMethods.GetMonitorBrightness(hPhysicalMonitor, ref pdwMinimumBrightness, ref pdwCurrentBrightness, ref pdwMaximumBrightness);

			return ret;
		}

		#endregion Brightness

		#region Contrast

		private bool BackupContrast()
		{
			int pdwMinimumContrast = 0;
			int pdwCurrentContrast = 0;
			int pdwMaximumContrast = 0;

			bool ret = GetContrast(ref pdwMinimumContrast, ref pdwCurrentContrast, ref pdwMaximumContrast);
			if (ret)
			{
				contrastSettings.MinimumContrast = pdwMinimumContrast;
				contrastSettings.CurrentContrast = pdwCurrentContrast;
				contrastSettings.MaximumContrast = pdwMaximumContrast;
				contrastSettings.SettingsSet();
			}
			return ret;
		}

		private async Task<bool> RestoreContrast()
		{
			if (!contrastSettings.AreSettingsSet())
				return false;

			bool ret = await SetContrast(contrastSettings.CurrentContrast);

			return ret;
		}

		public async Task<bool> SetContrast(int contrast)
		{
			if (!HasMonitorCapabilitiesContrast())
				return false;

			int oldContrast = GetContrast();

			int adjContrast = Math.Max(Math.Min(contrast, contrastSettings.MaximumContrast), contrastSettings.MinimumContrast);

			bool ret = NativeMethods.SetMonitorContrast(hPhysicalMonitor, adjContrast);
			if (!ret)
				return false;

			int newContrast = GetContrast();
			int counter = 0;
			while (newContrast == -1 || counter > 10)
			{
				await Task.Delay(10);
				newContrast = GetContrast();
				counter++;
			};

			Debug.WriteLine($"Org Contrast:{contrast}, Adjusted Contrast:{adjContrast}, oldContrast:{oldContrast}, newContrast:{newContrast} on device:{hPhysicalMonitor}");

			return newContrast != -1;
		}

		public bool GetContrast(ref int pdwMinimumContrast, ref int pdwCurrentContrast, ref int pdwMaximumContrast)
		{
			if (!HasMonitorCapabilitiesContrast())
				return false;

			bool ret = NativeMethods.GetMonitorContrast(hPhysicalMonitor, ref pdwMinimumContrast, ref pdwCurrentContrast, ref pdwMaximumContrast);

			Debug.WriteLine($"GetContrast - Monitor:{hPhysicalMonitor}, Min:{pdwMinimumContrast}, Max:{pdwMaximumContrast}, Current:{pdwCurrentContrast}");

			return ret;
		}

		public int GetContrast()
		{
			if (!HasMonitorCapabilitiesContrast())
				return -1;

			int pdwMinimumContrast = 0;
			int pdwCurrentContrast = 0;
			int pdwMaximumContrast = 0;

			bool ret = GetContrast(ref pdwMinimumContrast, ref pdwCurrentContrast, ref pdwMaximumContrast);
			if (ret)
				return pdwCurrentContrast;

			return -1;
		}

		#endregion Contrast
	}
}