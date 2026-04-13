using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MealShareDotNet.Client.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public class PageChangeEventArgs : EventArgs
    {
        public ObservableObject NextPage { get; set; } = default!;
    }

    public event EventHandler<PageChangeEventArgs>? PageChangeEventHandler = default!;
}
