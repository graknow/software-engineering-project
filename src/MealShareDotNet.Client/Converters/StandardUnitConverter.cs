using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

public class StandardUnitConverter : IValueConverter
{
    public static readonly StandardUnitConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is long standardMeasure && parameter is string unit
            && targetType.IsAssignableTo(typeof(long)))
        {
            return 0;
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}