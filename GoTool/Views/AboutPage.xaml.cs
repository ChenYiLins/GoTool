﻿using GoTool.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace GoTool.Views;

public sealed partial class AboutPage : Page
{
    public AboutViewModel ViewModel
    {
        get;
    }

    public AboutPage()
    {
        ViewModel = App.GetService<AboutViewModel>();
        InitializeComponent();
    }
}
