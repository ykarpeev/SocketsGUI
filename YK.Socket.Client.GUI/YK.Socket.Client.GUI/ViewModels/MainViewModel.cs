// <copyright file="MainViewModel.cs" company="Youri">
// Copyright (c) Youri. All rights reserved.
// </copyright>

namespace YK.Socket.Client.GUI.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;
    using Avalonia.Threading;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using YK.Socket.Client.GUI.Models;
    using YK.Socket.Comms.TCP;
    using YK.Socket.Extensions;

    /// <summary>
    /// Main view model.
    /// </summary>
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<LogMessage> messages;

        private SocketClient? socket;

        [ObservableProperty]
        private string host = "127.0.0.1";

        [ObservableProperty]
        private int port = 1000;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SendBufferCommand))]
        private string outgoingBuffer;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
        private bool isConnecting;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
        [NotifyCanExecuteChangedFor(nameof(SendBufferCommand))]
        private bool isConnected;

        private bool reconnect = false;

        [ObservableProperty]
        private bool appendCR;

        [ObservableProperty]
        private bool appendNL;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SendBufferCommand))]
        private bool sendHex;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            this.Messages = [];
            this.OutgoingBuffer = string.Empty;
            this.Messages.Add(new LogMessage("Ready", DateTime.Now, EMessageType.Connection));
        }

        [RelayCommand(CanExecute = nameof(CanSend))]
        private void SendBuffer()
        {
            var s = this.OutgoingBuffer;
            if (this.AppendCR)
            {
                s += "\r";
            }

            if (this.AppendNL)
            {
                s += "\n";
            }

            var b = Encoding.UTF8.GetBytes(s);

            if (this.SendHex)
            {
                b = ArrayHelpers.HexToByteArray(s);
            }

            if (this.IsConnected)
            {
                if (b.Length != 0)
                {
                    this.socket?.Send(b);

                    this.Messages.Add(new LogMessage(b, DateTime.Now, EMessageType.Outgoing));
                }
            }
        }

        [RelayCommand]
        private void Clear()
        {
            this.Messages?.Clear();
        }

        [RelayCommand(CanExecute = nameof(CanConnect))]
        private void Connect()
        {
            if (this.socket != null)
            {
                if (this.IsConnected)
                {
                    this.socket.Disconnect();
                    this.reconnect = true;
                    return;
                }
            }

            this.IsConnecting = true;

            this.socket = new SocketClient();
            this.socket.ErrorEvent += this.Socket_ErrorEvent;
            this.socket.DisconnectedEvent += this.Socket_DisconnectedEvent;
            this.socket.ConnectedEvent += this.Socket_ConnectedEvent;
            this.socket.DataReceivedEvent += this.Socket_DataReceivedEvent;
            this.socket.Connect(this.Host, this.Port);
        }

        private bool CanConnect()
        {
            if (this.IsConnecting)
            {
                return false;
            }

            return true;
        }

        private bool CanSend()
        {
            bool connected = this.IsConnected && this.OutgoingBuffer != null;
            bool validData = true;

            if (this.SendHex)
            {
                if (this.OutgoingBuffer != null)
                {
                    validData = this.OutgoingBuffer.IsValidHex();
                }
                else
                {
                    validData = false;
                }
            }

            if (validData && connected)
            {
                return true;
            }

            return false;
        }

        private void Socket_ConnectedEvent(object? sender, bool e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                this.IsConnecting = false;
                this.IsConnected = true;
                this.Messages.Add(new LogMessage("Connected", DateTime.Now, EMessageType.Connection));
            });
        }

        private void Socket_DataReceivedEvent(object? sender, byte[] e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                this.Messages.Add(new LogMessage(e, DateTime.Now, EMessageType.Incoming));
            });
        }

        private void Socket_DisconnectedEvent(object? sender, bool e)
        {
            void Disconnected()
            {
                if (this.IsConnected)
                {
                    this.Messages.Add(new LogMessage("Disconnected", DateTime.Now, EMessageType.Connection));
                }

                this.IsConnecting = false;
                this.IsConnected = false;

                if (this.reconnect)
                {
                    this.reconnect = false;
                    this.ConnectCommand.Execute(this);
                }
            }

            Dispatcher.UIThread.Post(() => Disconnected());
        }

        private void Socket_ErrorEvent(object? sender, string e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                this.Messages.Add(new LogMessage(e, DateTime.Now, EMessageType.Error));
            });
        }
    }
}
