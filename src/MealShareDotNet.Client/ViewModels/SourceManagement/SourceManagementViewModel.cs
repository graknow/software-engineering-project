using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MealShareDotNet.Client.ViewModels.SourceManagement;

public partial class SourceManagementViewModel : ViewModelBase
{
    private readonly IServiceProvider _services;

    public IEnumerable<IRecipeService> RecipeServices => _services.GetServices<IRecipeService>();

    public SourceManagementViewModel(IServiceProvider services)
    {
        _services = services;
    }
}