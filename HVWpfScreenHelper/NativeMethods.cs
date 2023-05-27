using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace HVWpfScreenHelper
{
	internal static class NativeMethods
	{
		public delegate bool MonitorEnumProc(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lParam);

		public enum DpiType
		{
			EFFECTIVE,
			ANGULAR,
			RAW
		}

		public enum SystemMetric
		{
			SM_CXSCREEN = 0,
			SM_CYSCREEN = 1,
			SM_XVIRTUALSCREEN = 76,
			SM_YVIRTUALSCREEN = 77,
			SM_CXVIRTUALSCREEN = 78,
			SM_CYVIRTUALSCREEN = 79,
			SM_CMONITORS = 80
		}

		public enum SPI : uint
		{
			SPI_GETWORKAREA = 48u
		}

		[Flags]
		public enum SPIF
		{
			None = 0x0,
			SPIF_UPDATEINIFILE = 0x1,
			SPIF_SENDCHANGE = 0x2,
			SPIF_SENDWININICHANGE = 0x2
		}

		public enum MonitorDefault
		{
			MONITOR_DEFAULTTONEAREST = 2,
			MONITOR_DEFAULTTONULL = 0,
			MONITOR_DEFAULTTOPRIMARY = 1
		}

		public enum D2D1_FACTORY_TYPE
		{
			D2D1_FACTORY_TYPE_SINGLE_THREADED,
			D2D1_FACTORY_TYPE_MULTI_THREADED
		}

		public struct RECT
		{
			public int left;

			public int top;

			public int right;

			public int bottom;

			public Size Size => new Size(right - left, bottom - top);

			public RECT(int left, int top, int right, int bottom)
			{
				this.left = left;
				this.top = top;
				this.right = right;
				this.bottom = bottom;
			}

			public RECT(Rect r)
			{
				left = (int)r.Left;
				top = (int)r.Top;
				right = (int)r.Right;
				bottom = (int)r.Bottom;
			}

			public static RECT FromXYWH(int x, int y, int width, int height)
			{
				return new RECT(x, y, x + width, y + height);
			}
		}

		public struct POINTSTRUCT
		{
			public int x;

			public int y;

			public POINTSTRUCT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public class POINT
		{
			public int x;

			public int y;

			public POINT()
			{
			}

			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
		public class MONITORINFOEX
		{
			internal int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));

			internal RECT rcMonitor;

			internal RECT rcWork;

			internal int dwFlags;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			internal char[] szDevice = new char[32];
		}

		[StructLayout(LayoutKind.Sequential)]
		public class COMRECT
		{
			public int bottom;

			public int left;

			public int right;

			public int top;

			public COMRECT()
			{
			}

			public COMRECT(Rect r)
			{
				left = (int)r.X;
				top = (int)r.Y;
				right = (int)r.Right;
				bottom = (int)r.Bottom;
			}

			public COMRECT(int left, int top, int right, int bottom)
			{
				this.left = left;
				this.top = top;
				this.right = right;
				this.bottom = bottom;
			}

			public static COMRECT FromXYWH(int x, int y, int width, int height)
			{
				return new COMRECT(x, y, x + width, y + height);
			}

			public override string ToString()
			{
				return "Left = " + left + " Top " + top + " Right = " + right + " Bottom = " + bottom;
			}
		}

		[Flags]
		public enum MonitorCapabilitiesMask
		{
			MC_CAPS_NONE = 0x00000000,
			MC_CAPS_MONITOR_TECHNOLOGY_TYPE = 0x00000001,
			MC_CAPS_BRIGHTNESS = 0x00000002,
			MC_CAPS_CONTRAST = 0x00000004,
			MC_CAPS_COLOR_TEMPERATURE = 0x00000008,
			MC_CAPS_RED_GREEN_BLUE_GAIN = 0x00000010,
			MC_CAPS_RED_GREEN_BLUE_DRIVE = 0x00000020,
			MC_CAPS_DEGAUSS = 0x00000040,
			MC_CAPS_DISPLAY_AREA_POSITION = 0x00000080,
			MC_CAPS_DISPLAY_AREA_SIZE = 0x00000100,
			MC_CAPS_RESTORE_FACTORY_DEFAULTS = 0x00000400,
			MC_CAPS_RESTORE_FACTORY_COLOR_DEFAULTS = 0x00000800,
			MC_RESTORE_FACTORY_DEFAULTS_ENABLES_MONITOR_SETTINGS = 0x00001000
		}

		[Flags]
		public enum ColorTemperatureMask
		{
			MC_SUPPORTED_COLOR_TEMPERATURE_NONE = 0x00000000,
			MC_SUPPORTED_COLOR_TEMPERATURE_4000K = 0x00000001,
			MC_SUPPORTED_COLOR_TEMPERATURE_5000K = 0x00000002,
			MC_SUPPORTED_COLOR_TEMPERATURE_6500K = 0x00000004,
			MC_SUPPORTED_COLOR_TEMPERATURE_7500K = 0x00000008,
			MC_SUPPORTED_COLOR_TEMPERATURE_8200K = 0x00000010,
			MC_SUPPORTED_COLOR_TEMPERATURE_9300K = 0x00000020,
			MC_SUPPORTED_COLOR_TEMPERATURE_10000K = 0x00000040,
			MC_SUPPORTED_COLOR_TEMPERATURE_11500K = 0x00000080,
		}

		[DllImport("Dxva2.dll")]
		public static extern bool GetMonitorCapabilities(
			/*[in] HANDLE*/ IntPtr hMonitor,
			/*[out] LPDWORD*/ ref MonitorCapabilitiesMask pdwMonitorCapabilities,
			/*[out] LPDWORD*/ ref ColorTemperatureMask pdwSupportedColorTemperatures);

		[DllImport("Dxva2.dll")]
		public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(
			/*[in] HMONITOR*/ IntPtr hMonitor,
			/*[out] LPDWORD*/ ref int pdwNumberOfPhysicalMonitors);

		public static int PHYSICAL_MONITOR_DESCRIPTION_SIZE = 128;

		[StructLayout(LayoutKind.Sequential)]
		public struct PHYSICAL_MONITOR
		{
			/*[in] HMONITOR*/
			public IntPtr hPhysicalMonitor;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public System.Char[] szPhysicalMonitorDescription;
		};

		[DllImport("Dxva2.dll")]
		public static extern bool GetPhysicalMonitorsFromHMONITOR(
			/*[in] HMONITOR*/ IntPtr hMonitor,
			/*[in] DWORD */ int dwPhysicalMonitorArraySize,
			/*[out] LPPHYSICAL_MONITOR*/ ref PHYSICAL_MONITOR pPhysicalMonitorArray);

		[DllImport("Dxva2.dll")]
		public static extern bool GetMonitorContrast(
			/*[in] HMONITOR*/ IntPtr hMonitor,
			/*[out] LPDWORD*/ ref int pdwMinimumContrast,
			/*[out] LPDWORD*/ ref int pdwCurrentContrast,
			/*[out] LPDWORD*/ ref int pdwMaximumContrast);

		[DllImport("Dxva2.dll")]
		public static extern bool GetMonitorBrightness(
			/*[in] HMONITOR*/ IntPtr hMonitor,
			/*[out] LPDWORD*/ ref int pdwMinimumBrightness,
			/*[out] LPDWORD*/ ref int pdwCurrentBrightness,
			/*[out] LPDWORD*/ ref int pdwMaximumBrightness);

		[DllImport("Dxva2.dll")]
		public static extern bool SetMonitorBrightness(
			/*[in] HMONITOR*/ IntPtr hMonitor,
			/*[in] DWORD*/ int dwNewBrightness);

		[DllImport("Dxva2.dll")]
		public static extern bool SetMonitorContrast(
			/*[in] HMONITOR*/ IntPtr hMonitor,
			/*[in] DWORD*/ int dwNewContrast);

		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[Guid("06152247-6f50-465a-9245-118bfd3b6007")]
		internal interface ID2D1Factory
		{
			int ReloadSystemMetrics();

			[PreserveSig]
			void GetDesktopDpi(out float dpiX, out float dpiY);
		}

		public static readonly HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);

		[DllImport("shcore.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, out uint dpiX, out uint dpiY);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool GetMonitorInfo(HandleRef hmonitor, [In][Out] MONITORINFOEX info);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern bool EnumDisplayMonitors(HandleRef hdc, COMRECT rcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern IntPtr MonitorFromWindow(HandleRef handle, int flags);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int GetSystemMetrics(SystemMetric nIndex);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool SystemParametersInfo(SPI nAction, int nParam, ref RECT rc, SPIF nUpdate);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern IntPtr MonitorFromPoint(POINTSTRUCT pt, MonitorDefault flags);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool GetCursorPos([In][Out] POINT pt);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool IsProcessDPIAware();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("d2d1.dll")]
		public static extern int D2D1CreateFactory(D2D1_FACTORY_TYPE factoryType, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, IntPtr pFactoryOptions, out ID2D1Factory ppIFactory);
	}
}