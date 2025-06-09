// <copyright file="MessageConverter.cs" company="Youri">
// Copyright (c) Youri. All rights reserved.
// </copyright>

namespace YK.Socket.Client.GUI
{
    using System;
    using System.Globalization;
    using Avalonia.Data.Converters;
    using Avalonia.Media;
    using YK.Socket.Client.GUI.Models;

    public class MessageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Brushes.Black;
            }

            var status = (EMessageType)value;

            return status switch
            {
                EMessageType.Connection => Brushes.DarkGray,
                EMessageType.Error => Brushes.Red,
                EMessageType.Outgoing => Brushes.Blue,
                EMessageType.Incoming => Brushes.DarkBlue,
                _ => Brushes.Green
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
