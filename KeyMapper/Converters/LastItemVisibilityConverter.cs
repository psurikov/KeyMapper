﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KeyMapper.Converters
{
    public class LastItemVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 &&
                values[0] is int index &&
                values[1] is int count)
                return (index == count - 1) ? Visibility.Collapsed : Visibility.Visible;
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}