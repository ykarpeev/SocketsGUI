// <copyright file="EMessageType.cs" company="Youri">
// Copyright (c) Youri. All rights reserved.
// </copyright>

namespace YK.Socket.Client.GUI.Models
{
    /// <summary>
    /// Message type enum.
    /// </summary>
    public enum EMessageType
    {
        /// <summary>
        /// Connection related - disconnects/connects.
        /// </summary>
        Connection,

        /// <summary>
        /// Error.
        /// </summary>
        Error,

        /// <summary>
        /// Incoming data.
        /// </summary>
        Incoming,

        /// <summary>
        /// Outgoing data.
        /// </summary>
        Outgoing,
    }
}
