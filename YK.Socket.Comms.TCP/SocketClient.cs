// <copyright file="SocketClient.cs" company="Youri">
// Copyright (c) Youri. All rights reserved.
// </copyright>

namespace YK.Socket.Comms.TCP
{
    using System.Net.Sockets;
    using SuperSimpleTcp;
    using YK.Socket.Comms.Services;

    /// <summary>
    /// Socket client class - using supersimpleTCP
    /// </summary>
    public class SocketClient : IComms
    {
        private SimpleTcpClient? client;

        /// <inheritdoc/>
        public event EventHandler<bool>? ConnectedEvent;

        /// <inheritdoc/>
        public event EventHandler<bool>? DisconnectedEvent;

        /// <inheritdoc/>
        public event EventHandler<string>? ErrorEvent;

        /// <inheritdoc/>
        public event EventHandler<byte[]>? DataReceivedEvent;

        /// <inheritdoc/>
        public void Connect(string host, int port)
        {
            this.client = new SimpleTcpClient(host, port);
            this.client.Events.Connected += this.Events_Connected;
            this.client.Events.Disconnected += this.Events_Disconnected;
            this.client.Events.DataReceived += this.Events_DataReceived;
            Task.Run(() => this.EstablishConnection());
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            this.client?.Disconnect();
        }

        /// <inheritdoc/>
        public void Send(byte[] data)
        {
            if (data.Length != 0)
            {
                if (this.client != null)
                {
                    if (this.client.IsConnected)
                    {
                        this.client.Send(data);
                    }
                }
            }
        }

        private void EstablishConnection()
        {
            try
            {
                this.client?.Connect();
            }
            catch (SocketException ex)
            {
                this.ErrorEvent?.Invoke(this, ex.Message);
                this.DisconnectedEvent?.Invoke(this, true);
            }
            catch (System.TimeoutException ex)
            {
                this.ErrorEvent?.Invoke(this, ex.Message);
                this.DisconnectedEvent?.Invoke(this, true);
            }
        }

        private void Events_Disconnected(object? sender, ConnectionEventArgs e)
        {
            this.DisconnectedEvent?.Invoke(this, true);
        }

        private void Events_Connected(object? sender, ConnectionEventArgs e)
        {
            this.ConnectedEvent?.Invoke(this, true);
        }

        private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
        {
            if (e != null)
            {
                if (e.Data.Array != null)
                {
                    this.DataReceivedEvent?.Invoke(this, e.Data.Array[..e.Data.Count]);
                }
            }
        }
    }
}
