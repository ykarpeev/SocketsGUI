// <copyright file="SocketClient.cs" company="Youri">
// Copyright (c) Youri. All rights reserved.
// </copyright>

namespace YK.Socket.Comms.TCP
{
    using System.Net.Sockets;
    using SuperSimpleTcp;
    using YK.Socket.Comms.Services;

    /// <summary>
    /// Socket client class - using supersimpleTCP.
    /// </summary>
    public class SocketClient : IComms, IDisposable
    {
        private readonly object connectLock = new object();
        private SimpleTcpClient? client;
        private volatile bool disposed;

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
            if (string.IsNullOrWhiteSpace(host))
            {
                this.ErrorEvent?.Invoke(this, "Host cannot be empty.");
                this.DisconnectedEvent?.Invoke(this, true);
                return;
            }

            if (port < 1 || port > 65535)
            {
                this.ErrorEvent?.Invoke(this, $"Port {port} is out of the valid range (1-65535).");
                this.DisconnectedEvent?.Invoke(this, true);
                return;
            }

            lock (this.connectLock)
            {
                this.DisposeClient();
                this.client = new SimpleTcpClient(host, port);
                this.client.Events.Connected += this.Events_Connected;
                this.client.Events.Disconnected += this.Events_Disconnected;
                this.client.Events.DataReceived += this.Events_DataReceived;
            }

            Task.Run(() => this.EstablishConnection());
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            lock (this.connectLock)
            {
                if (this.client == null)
                {
                    this.ErrorEvent?.Invoke(this, "Cannot disconnect: no active connection.");
                    return;
                }

                this.client.Disconnect();
            }
        }

        /// <inheritdoc/>
        public void Send(byte[] data)
        {
            if (data?.Length > 0 && this.client?.IsConnected == true)
            {
                this.client.Send(data);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and optionally managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                lock (this.connectLock)
                {
                    this.DisposeClient();
                }
            }

            this.disposed = true;
        }

        private void DisposeClient()
        {
            if (this.client != null)
            {
                this.client.Events.Connected -= this.Events_Connected;
                this.client.Events.Disconnected -= this.Events_Disconnected;
                this.client.Events.DataReceived -= this.Events_DataReceived;
                this.client.Dispose();
                this.client = null;
            }
        }

        private void EstablishConnection()
        {
            try
            {
                SimpleTcpClient? current;
                lock (this.connectLock)
                {
                    current = this.client;
                }

                current?.Connect();
            }
            catch (SocketException ex)
            {
                this.ErrorEvent?.Invoke(this, ex.Message);
                this.DisconnectedEvent?.Invoke(this, true);
            }
            catch (TimeoutException ex)
            {
                this.ErrorEvent?.Invoke(this, ex.Message);
                this.DisconnectedEvent?.Invoke(this, true);
            }
            catch (OperationCanceledException ex)
            {
                this.ErrorEvent?.Invoke(this, ex.Message);
                this.DisconnectedEvent?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                this.ErrorEvent?.Invoke(this, $"Unexpected error: {ex.Message}");
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
            var data = e?.Data.Array;
            if (data != null && e != null)
            {
                this.DataReceivedEvent?.Invoke(this, data[..e.Data.Count]);
            }
        }
    }
}
