using System;
using System.Windows.Data;

namespace BlankScreen2.Converts
{
	public sealed class TimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is DateTime dateTime))
				return string.Empty;

			return dateTime.ToString("hh:mm:ss tt");
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}