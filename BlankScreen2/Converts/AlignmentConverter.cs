using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BlankScreen2.Model;

namespace BlankScreen2.Converts
{
	internal sealed class HorizontalAlignmentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Location location))
				return HorizontalAlignment.Left;

			switch (location)
			{
				case Location.TopLeft:
				case Location.BottomLeft:
					return HorizontalAlignment.Left;

				case Location.TopRight:
				case Location.BottomRight:
					return HorizontalAlignment.Right;

				default:
					return HorizontalAlignment.Left;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	internal sealed class VerticalAlignmentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Location location))
				return VerticalAlignment.Top;

			switch (location)
			{
				case Location.TopLeft:
				case Location.TopRight:
					return VerticalAlignment.Top;

				case Location.BottomLeft:
				case Location.BottomRight:
					return VerticalAlignment.Bottom;

				default:
					return VerticalAlignment.Top;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}