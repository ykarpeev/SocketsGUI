// <copyright file="App.axaml.cs" company="Youri">
// Copyright (c) Youri. All rights reserved.
// </copyright>

namespace YK.Socket.Client.GUI
{
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using AvaloniaUI.DiagnosticsSupport;
    using YK.Socket.Client.GUI.ViewModels;
    using YK.Socket.Client.GUI.Views;

    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            this.AttachDeveloperTools();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(),
                };
            }
            else if (this.ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = new MainViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}