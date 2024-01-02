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
		private IntPtr physicalMonitorHandle = IntPtr.MinValue;

		//private NativeMethods.MonitorCapabilitiesMask monitorCababilities = MonitorCapabilitiesMask.MC_CAPS_NONE;

		private MonitorSettings? _monitorCapabilites;

		public static IEnumerable<Screen> AllScreens
		{
			get
			{
				if (MultiMonitorSupport)
				{
					MonitorEnumCallback monitorEnumCallback = new MonitorEnumCallback();
					NativeMethods.MonitorEnumProc lpfnEnum = monitorEnumCallback.Callback;
					EnumDisplayMonitors(NullHandleRef, null, lpfnEnum, IntPtr.Zero);
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

				Screen screen = AllScreens.FirstOrDefault(t => t.Primary, new Screen((IntPtr)(-1163005939)));

				return screen;
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
					SystemParametersInfo(SPI.SPI_GETWORKAREA, 0, ref rc, SPIF.SPIF_SENDCHANGE);
					result = new Rect(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
				}
				else
				{
					NativeMethods.MONITORINFOEX mONITORINFOEX = new NativeMethods.MONITORINFOEX();
					GetMonitorInfo(new HandleRef(null, monitorHandle), mONITORINFOEX);
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

		protected IntPtr MonitorHandle { get => monitorHandle; }

		public MonitorSettings MonitorCapabilites
		{
			get
			{
				if (_monitorCapabilites == null)
					_monitorCapabilites = new MonitorSettings(this);

				return _monitorCapabilites;
			}
			set => _monitorCapabilites = value;
		}

		static Screen()
		{
			MultiMonitorSupport = GetSystemMetrics(SystemMetric.SM_CMONITORS) != 0;
		}

		private Screen(IntPtr monitor)
			: this(monitor, IntPtr.Zero)
		{
		}

		private Screen(IntPtr monitor, IntPtr hdc)
		{
			if (IsProcessDPIAware())
			{
				uint dpiX;
				try
				{
					uint dpiY;
					if (monitor == (IntPtr)(-1163005939))
					{
						GetDpiForMonitor(MonitorFromPoint(new NativeMethods.POINTSTRUCT(0, 0), MonitorDefault.MONITOR_DEFAULTTOPRIMARY), DpiType.EFFECTIVE, out dpiX, out dpiY);
					}
					else
					{
						GetDpiForMonitor(monitor, DpiType.EFFECTIVE, out dpiX, out dpiY);
					}
				}
				catch
				{
					if (D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, typeof(NativeMethods.ID2D1Factory).GUID, IntPtr.Zero, out var ppIFactory) < 0)
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
				Size size = new Size(GetSystemMetrics(SystemMetric.SM_CXSCREEN), GetSystemMetrics(SystemMetric.SM_CYSCREEN));
				Bounds = new Rect(0.0, 0.0, size.Width, size.Height);
				Primary = true;
				DeviceName = "DISPLAY";
			}
			else
			{
				NativeMethods.MONITORINFOEX mONITORINFOEX = new NativeMethods.MONITORINFOEX();
				GetMonitorInfo(new HandleRef(null, monitor), mONITORINFOEX);
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

			return new Screen(MonitorFromWindow(new HandleRef(null, hwnd), 2));
		}

		public static Screen FromPoint(Point point)
		{
			if (MultiMonitorSupport)
			{
				return new Screen(MonitorFromPoint(new NativeMethods.POINTSTRUCT((int)point.X, (int)point.Y), MonitorDefault.MONITOR_DEFAULTTONEAREST));
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

		#region Monitor Capabilities

		public bool RefreshMonitorCapabilities()
		{
			bool ret = false;
			if (physicalMonitorHandle == IntPtr.MinValue)
			{
				int monitorCount = 0;
				ret = GetNumberOfPhysicalMonitorsFromHMONITOR(monitorHandle, ref monitorCount);
				if (!ret || monitorCount != 1)
					return false;

				PHYSICAL_MONITOR pHYSICAL_MONITOR = new PHYSICAL_MONITOR
				{
					szPhysicalMonitorDescription = new System.Char[PHYSICAL_MONITOR_DESCRIPTION_SIZE]
				};

				ret = GetPhysicalMonitorsFromHMONITOR(MonitorHandle, monitorCount, ref pHYSICAL_MONITOR);
				if (!ret)
					return false;

				physicalMonitorHandle = pHYSICAL_MONITOR.hPhysicalMonitor;
			}

			MonitorCapabilitiesMask monitorCababilities = MonitorCapabilitiesMask.MC_CAPS_NONE;
			ColorTemperatureMask colorTemperature = ColorTemperatureMask.MC_SUPPORTED_COLOR_TEMPERATURE_NONE;

			ret = GetMonitorCapabilities(physicalMonitorHandle, ref monitorCababilities, ref colorTemperature);
			if (ret)
			{
				MonitorCapabilites.MonitorCababilities = monitorCababilities;
				MonitorCapabilites.ColorTemperature = colorTemperature;
			}

			return ret;
		}

		public bool HasMonitorCapabilitiesBrightness()
		{
			return MonitorCapabilites.MC_CAPS_BRIGHTNESS;
		}

		public bool HasMonitorCapabilitiesContrast()
		{
			return MonitorCapabilites.MC_CAPS_CONTRAST;
		}

		#endregion Monitor Capabilities

		#region backup and restore

		public void BackupMonitorCapabilities()
		{
			if (HasMonitorCapabilitiesBrightness())
				BackupBrightness(MonitorCapabilites.BrightnessSettings);

			if (HasMonitorCapabilitiesContrast())
				BackupContrast(MonitorCapabilites.ContrastSettings);
		}

		public async Task RestoreMonitorCapabilities()
		{
			if (HasMonitorCapabilitiesBrightness())
				await RestoreBrightness(MonitorCapabilites.BrightnessSettings);

			if (HasMonitorCapabilitiesContrast())
				await RestoreContrast(MonitorCapabilites.ContrastSettings);
		}

		public bool NeedsRestore()
		{
			int brightness = GetBrightness();
			if (brightness != MonitorCapabilites.BrightnessSettings.CurrentBrightness)
				return true;

			int contrast = GetContrast();
			if (contrast != MonitorCapabilites.ContrastSettings.CurrentContrast)
				return true;

			return false;
		}

		#endregion backup and restore

		#region Brightness

		private bool BackupBrightness(BrightnessSettings brightnessSettings)
		{
			int pdwMinimumBrightness = 0;
			int pdwCurrentBrightness = 0;
			int pdwMaximumBrightness = 0;
			bool ret = GetMonitorBrightness(physicalMonitorHandle, ref pdwMinimumBrightness, ref pdwCurrentBrightness, ref pdwMaximumBrightness);
			Debug.Assert(ret);
			if (ret)
			{
				brightnessSettings.SetSettings(pdwMinimumBrightness, pdwCurrentBrightness, pdwMaximumBrightness);
			}

			return ret;
		}

		private async Task<bool> RestoreBrightness(BrightnessSettings brightnessSettings)
		{
			if (!brightnessSettings.SettingsSet)
				return false;

			bool ret = await SetBrightness(brightnessSettings.CurrentBrightness);

			return ret;
		}

		public async Task<bool> SetBrightness(int brightness)
		{
			if (!HasMonitorCapabilitiesBrightness())
				return false;

			int oldBrightness = GetBrightness();

			brightness = Math.Max(Math.Min(brightness, MonitorCapabilites.BrightnessSettings.MaximumBrightness), MonitorCapabilites.BrightnessSettings.MinimumBrightness);
			bool ret = SetMonitorBrightness(physicalMonitorHandle, brightness);
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
			bool ret = GetMonitorBrightness(physicalMonitorHandle, ref pdwMinimumBrightness, ref pdwCurrentBrightness, ref pdwMaximumBrightness);

			return ret;
		}

		#endregion Brightness

		#region Contrast

		private bool BackupContrast(ContrastSettings contrastSettings)
		{
			int pdwMinimumContrast = 0;
			int pdwCurrentContrast = 0;
			int pdwMaximumContrast = 0;

			bool ret = GetContrast(ref pdwMinimumContrast, ref pdwCurrentContrast, ref pdwMaximumContrast);
			Debug.Assert(ret);
			if (ret)
			{
				contrastSettings.MinimumContrast = pdwMinimumContrast;
				contrastSettings.CurrentContrast = pdwCurrentContrast;
				contrastSettings.MaximumContrast = pdwMaximumContrast;
				contrastSettings.SettingsSet();
			}
			return ret;
		}

		private async Task<bool> RestoreContrast(ContrastSettings contrastSettings)
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

			int adjContrast = Math.Max(Math.Min(contrast, MonitorCapabilites.ContrastSettings.MaximumContrast), MonitorCapabilites.ContrastSettings.MinimumContrast);

			bool ret = SetMonitorContrast(physicalMonitorHandle, adjContrast);
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

			Debug.WriteLine($"Org Contrast:{contrast}, Adjusted Contrast:{adjContrast}, oldContrast:{oldContrast}, newContrast:{newContrast} on device:{physicalMonitorHandle}");

			return newContrast != -1;
		}

		public bool GetContrast(ref int pdwMinimumContrast, ref int pdwCurrentContrast, ref int pdwMaximumContrast)
		{
			if (!HasMonitorCapabilitiesContrast())
				return false;

			bool ret = GetMonitorContrast(physicalMonitorHandle, ref pdwMinimumContrast, ref pdwCurrentContrast, ref pdwMaximumContrast);

			Debug.WriteLine($"GetContrast - Monitor:{physicalMonitorHandle}, Min:{pdwMinimumContrast}, Max:{pdwMaximumContrast}, Current:{pdwCurrentContrast}");

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