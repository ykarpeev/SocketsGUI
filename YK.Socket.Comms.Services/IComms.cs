// <copyright file="IComms.cs" company="Youri">
// Copyright (c) Youri. All rights reserved.
// </copyright>

namespace YK.Socket.Comms.Services
{
    /// <summary>
    /// Interface for communication over a tcp/socket or rs232.
    /// </summary>
    public interface IComms
    {
        /// <summary>
        /// Connected to server event.
        /// </summary>
        event EventHandler<bool>? ConnectedEvent;

        /// <summary>
        /// Disconnected from server event.
        /// </summary>
        event EventHandler<bool>? DisconnectedEvent;

        /// <summary>
        /// Error received event.
        /// </summary>
        event EventHandler<string>? ErrorEvent;

        /// <summary>
        /// Incoming data received.
        /// </summary>
        event EventHandler<byte[]> DataReceivedEvent;

        /// <summary>
        /// Connect to the server.
        /// </summary>
        /// <param name="host">Server IP/hostname..</param>
        /// <param name="port">Server port.</param>
        void Connect(string host, int port);

        /// <summary>
        /// Disconnect from server.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Send data to server.
        /// </summary>
        /// <param name="data">Byte array to send.</param>
        void Send(byte[] data);
    }
}
