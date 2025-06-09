using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace YK.Socket.Client.GUI;

/// <summary>
/// Auto scroll to bottom of datagrid behavior.
/// </summary>
public class AutoScrollBehavior : Behavior<DataGrid>
{
    private INotifyCollectionChanged? notifyCollectionChanged;
    private bool dataGridScrolledToEnd = true;

    /// <summary>
    /// Attaches the behavior to the <see cref="DataGrid"/>.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        this.AssociatedObject!.PropertyChanged += this.OnDataGridPropertyChanged;

        if (this.AssociatedObject!.IsLoaded)
        {
            this.AttachCollectionChanged();
        }
        else
        {
            this.AssociatedObject!.Loaded += this.OnLoaded;
        }
    }

    /// <summary>
    /// Detaches the behavior from the <see cref="DataGrid"/>.
    /// </summary>
    protected override void OnDetaching()
    {
        if (this.AssociatedObject != null)
        {
            this.AssociatedObject.PropertyChanged -= this.OnDataGridPropertyChanged;
        }

        this.DetachCollectionChanged();
        this.DetachScrollChanged();
        base.OnDetaching();
    }

    /// <summary>
    /// Handle data grid property changed event.
    /// </summary>
    /// <param name="sender">Source.</param>
    /// <param name="e">Event args.</param>
    private void OnDataGridPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name.Equals(nameof(DataGrid.ItemsSource)))
        {
            this.DetachCollectionChanged();
            this.AttachCollectionChanged();
        }
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        this.AssociatedObject!.Loaded -= this.OnLoaded;
        this.AttachScrollChanged();
    }

    private void AttachCollectionChanged()
    {
        this.notifyCollectionChanged = this.AssociatedObject!.ItemsSource as INotifyCollectionChanged;
        if (this.notifyCollectionChanged != null)
        {
            this.notifyCollectionChanged.CollectionChanged += this.ItemsCollectionChanged;
        }
    }

    private void DetachCollectionChanged()
    {
        if (this.notifyCollectionChanged != null)
        {
            this.notifyCollectionChanged.CollectionChanged -= this.ItemsCollectionChanged;
            this.notifyCollectionChanged = null;
        }
    }

    private void AttachScrollChanged()
    {
        var scrollViewer = this.GetVerticalScrollBar();
        if (scrollViewer != null)
        {
            scrollViewer.ValueChanged += this.OnScrollChanged;
        }
    }

    private void DetachScrollChanged()
    {
        var scrollViewer = this.GetVerticalScrollBar();
        if (scrollViewer != null)
        {
            scrollViewer.ValueChanged -= this.OnScrollChanged;
        }
    }

    private void OnScrollChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (sender is ScrollBar scrollBar)
        {
            this.dataGridScrolledToEnd = Math.Abs(scrollBar.Value - scrollBar.Maximum) < 80;
        }
    }

    private void ItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && this.dataGridScrolledToEnd)
        {
            this.AssociatedObject!.ScrollIntoView(e.NewItems![^1], null);
        }
    }

    /// <summary>
    /// Retrieves the vertical scroll bar of the <see cref="DataGrid"/>.
    /// </summary>
    /// <returns>The vertical <see cref="ScrollBar"/> of the <see cref="DataGrid"/>, if it exists.</returns>
    private ScrollBar? GetVerticalScrollBar()
    {
        return this.AssociatedObject?.GetVisualDescendants()
            .OfType<ScrollBar>()
            .FirstOrDefault(s => s.Name == "PART_VerticalScrollbar");
    }
}