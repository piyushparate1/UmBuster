using System;
using System.Windows;
using System.Windows.Input;
using UmBuster.ViewModels;

namespace UmBuster.Views;

public partial class OverlayWindow : Window
{
    private readonly OverlayViewModel _viewModel;

    public OverlayWindow()
    {
        InitializeComponent();
        _viewModel = new OverlayViewModel();
        DataContext = _viewModel;
        
        _viewModel.OpenSettingsRequested += OnOpenSettingsRequested;
        
        Loaded += (s, e) => _viewModel.Initialize();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void OnOpenSettingsRequested(object? sender, EventArgs e)
    {
        var settingsVm = _viewModel.CreateSettingsViewModel();
        var settingsWindow = new SettingsWindow(settingsVm)
        {
            Owner = this
        };
        settingsWindow.ShowDialog();
    }
}
