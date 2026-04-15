using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MealShareDotNet.Client.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public class PageChangeEventArgs : EventArgs
    {
        public Type NextViewType { get; set; } = default!;
        public Func<ViewModelBase, ViewModelBase> NextPageConfig { get; set; } = default!;
    }

    public event EventHandler<PageChangeEventArgs>? PageChangeEventHandler = default!;

    public void EmitPageChange(PageChangeEventArgs args)
    {
        PageChangeEventHandler?.Invoke(this, args);
    }
}
