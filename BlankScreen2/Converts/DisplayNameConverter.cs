using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BlankScreen2.Converts
{
	internal class DisplayNameConverter : IMultiValueConverter
	{
		private static string _Error = "Error";

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				return _Error;

			if (!(values[0] is string deviceName))
				return _Error;

			if (!(values[1] is Rect workingArea))
				return _Error;

			return ConvertDisplayName(deviceName, workingArea);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public static string ConvertDisplayName(string? deviceName, Rect workingArea)
		{
			StringBuilder sb = new();
			if (!string.IsNullOrEmpty(deviceName))
			{
				int pos = deviceName.LastIndexOf('\\') + 1;
				sb.Append(deviceName[pos..]);
			}
			sb.Append(" ");
			sb.Append(workingArea.Width);
			sb.Append("x");
			sb.Append(workingArea.Height);

			return sb.ToString();
		}
	}
}