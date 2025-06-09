// <copyright file="ViewLocator.cs" company="Youri">
// Copyright (c) Youri. All rights reserved.
// </copyright>

namespace YK.Socket.Client.GUI
{
    using System;
    using Avalonia.Controls;
    using Avalonia.Controls.Templates;
    using YK.Socket.Client.GUI.ViewModels;

    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? data)
        {
            if (data is null)
            {
                return null;
            }

            var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type) !;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}