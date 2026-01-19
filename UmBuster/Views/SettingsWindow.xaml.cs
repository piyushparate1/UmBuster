using System.Windows;
using System.Windows.Input;
using UmBuster.ViewModels;

namespace UmBuster.Views;

public partial class SettingsWindow : Window
{
    private readonly SettingsViewModel _viewModel;

    public SettingsWindow(SettingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        
        // Auto-focus the text box
        Loaded += (s, e) => Keyboard.Focus(NewWordBox);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void NewWordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _viewModel.AddWordCommand.Execute(null);
        }
    }
}
