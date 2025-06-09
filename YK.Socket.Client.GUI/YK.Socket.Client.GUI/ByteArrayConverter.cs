// <copyright file="ByteArrayConverter.cs" company="Youri">
// Copyright (c) Youri. All rights reserved.
// </copyright>

namespace YK.Socket.Client.GUI
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Avalonia.Data.Converters;

    /// <summary>
    /// Convert a byte array into a formatted hex string.
    /// </summary>
    public class ByteArrayConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var msg = (byte[])value;

            if (msg != null && msg.Length != 0)
            {
                string hex = string.Join(" ", msg.Select(x => x.ToString("X2")));

                return hex;
            }

            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
            throw new NotImplementedException();
        }
    }
}
