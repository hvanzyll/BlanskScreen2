using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlankScreen2.Converts
{
	internal sealed class SettingsVisablityConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool enabled))
				return Visibility.Collapsed;

			return enabled ? Visibility.Visible : Visibility.Collapsed;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}