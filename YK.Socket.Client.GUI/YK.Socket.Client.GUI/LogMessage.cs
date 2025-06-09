// <copyright file="LogMessage.cs" company="Youri">
// Copyright (c) Youri. All rights reserved.
// </copyright>

namespace YK.Socket.Client.GUI
{
    using System;
    using System.Text;
    using YK.Socket.Client.GUI.Models;

    /// <summary>
    /// Initialize a log message class.
    /// </summary>
    /// <param name="message">Message to show.</param>
    /// <param name="timeStamp">Message timetsmp.</param>
    /// <param name="type">Mesage type.</param>
    public class LogMessage(string message, DateTime timeStamp, EMessageType type)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="data">Data as a byte array.</param>
        /// <param name="ts">Message timestamp.</param>
        /// <param name="type">Message type.</param>
        public LogMessage(byte[] data, DateTime ts, EMessageType type)
            : this(Encoding.UTF8.GetString(data), ts, type)
        {
            this.RawData = data;
        }

        /// <summary>
        /// Gets the raw byte array.
        /// </summary>
        public byte[]? RawData { get; init; }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public EMessageType Type { get; init; } = type;

        /// <summary>
        /// Gets the message timestamp.
        /// </summary>
        public DateTime TimeStamp { get; init; } = timeStamp;

        /// <summary>
        /// Gets the mesage string.
        /// </summary>
        public string Message
        {
            get
            {
                return message.Replace("\r", "\\r").Replace("\n", "\\n");
            }
        }
    }
}
