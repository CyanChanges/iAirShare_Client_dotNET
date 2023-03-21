using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace iAirShare_Client.iAirShare;

public class TimestampConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Check if value is a double type
        if (value is float timestamp)
        {
            // Convert timestamp to long type
            var longTimestamp = (long)timestamp;

            // Convert timestamp to DateTime type
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(longTimestamp).LocalDateTime;

            // Check if parameter is a format string
            if (parameter is string format)
                // Convert dateTime to string type with format
                return dateTime.ToString(format);
        }

        // Return null if conversion fails
        return null!;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Check if value is a string type
        if (value is string text)
            // Check if parameter is a format string
            if (parameter is string format)
                // Try to parse text to DateTime type with format and culture
                if (DateTime.TryParseExact(text, format, culture,
                        DateTimeStyles.None, out var dateTime))
                    // Convert dateTime to long type as timestamp
                    return new DateTimeOffset(dateTime).ToUnixTimeSeconds();

        // Return null if conversion fails
        return null!;
    }
}

public class ByteSizeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ulong bytes)
        {
            string[] suffixes = { "Byte(s)", "KiB", "MiB", "GiB", "TiB", "PiB" };
            var counter = 0;
            double number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            if (counter < suffixes.Length) return $"{number:0.##} {suffixes[counter]}";
            counter = suffixes.Length - 1;
            return $"{number:0.##} {suffixes[counter]}";

        }

        return null!;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}