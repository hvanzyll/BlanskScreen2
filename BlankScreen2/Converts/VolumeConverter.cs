using System;
using System.Globalization;
using System.Windows.Data;

namespace BlankScreen2.Converts
{
	internal sealed class VolumeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is int volume))
				return "Error";

			return $"Volume: {volume}";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}