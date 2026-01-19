using System.Windows;
using UmBuster.ViewModels;
using UmBuster.Views;

namespace UmBuster;

public partial class App : System.Windows.Application
{
    private FlashWindow? _flash;
    private OverlayViewModel? _viewModel;

    private System.Windows.Forms.NotifyIcon? _notifyIcon;
    private System.Windows.Forms.ToolStripMenuItem? _startStopMenuItem;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Create flash window and view model
        _flash = new FlashWindow();
        _viewModel = new OverlayViewModel();

        // Setup System Tray Icon
        _notifyIcon = new System.Windows.Forms.NotifyIcon
        {
            Icon = System.Drawing.SystemIcons.Application, // Use default app icon
            Visible = true,
            Text = "UmBuster"
        };

        // Context Menu with 3 options: Settings, Start/Stop, Exit
        var contextMenu = new System.Windows.Forms.ContextMenuStrip();
        
        // Settings
        contextMenu.Items.Add("Settings", null, (s, args) => OpenSettings());
        
        // Start/Stop toggle
        _startStopMenuItem = new System.Windows.Forms.ToolStripMenuItem("Start", null, (s, args) => ToggleStartStop());
        contextMenu.Items.Add(_startStopMenuItem);
        
        // Exit
        contextMenu.Items.Add("Exit", null, (s, args) => Shutdown());
        
        // Update menu text when opening
        contextMenu.Opening += (s, args) => UpdateStartStopMenuText();
        
        _notifyIcon.ContextMenuStrip = contextMenu;

        // Wire up flash event
        if (_viewModel != null)
        {
            _viewModel.FillerDetected += (s, args) => _flash.Flash();
            _viewModel.Initialize();
        }

        // Show flash window only
        _flash.Show();
    }

    private void OpenSettings()
    {
        if (_viewModel == null) return;
        
        var settingsVm = _viewModel.CreateSettingsViewModel();
        var settingsWindow = new SettingsWindow(settingsVm);
        settingsWindow.ShowDialog();
    }

    private void ToggleStartStop()
    {
        _viewModel?.ToggleWrapperCommand.Execute(null);
        UpdateStartStopMenuText();
    }

    private void UpdateStartStopMenuText()
    {
        if (_startStopMenuItem != null && _viewModel != null)
        {
            _startStopMenuItem.Text = _viewModel.IsRunning ? "Stop" : "Start";
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _viewModel?.Dispose();
        _notifyIcon?.Dispose();
        base.OnExit(e);
    }
}
