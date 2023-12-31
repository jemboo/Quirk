﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Quirk.UI.W.Core.Models;
using Quirk.UI.W.Core.Models.Workspace;

namespace Quirk.UI.W.Views;

public sealed partial class ConfigsDetailControl : UserControl
{
    public CfgPlexVm? CfgPlex
    {
        get => GetValue(CfgPlexProperty) as CfgPlexVm;
        set => SetValue(CfgPlexProperty, value);
    }

    public static readonly DependencyProperty CfgPlexProperty = DependencyProperty.Register("CfgPlex", typeof(CfgPlexVm), typeof(ConfigsDetailControl), new PropertyMetadata(null, OnCfgPlexPropertyPropertyChanged));

    public ConfigsDetailControl()
    {
        InitializeComponent();
    }

    private static void OnCfgPlexPropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ConfigsDetailControl control)
        {
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
