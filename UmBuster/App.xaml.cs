using System.Windows;
using UmBuster.ViewModels;
using UmBuster.Views;

namespace UmBuster;

public partial class App : Application
{
    private OverlayWindow? _overlay;
    private FlashWindow? _flash;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Create windows
        _overlay = new OverlayWindow();
        _flash = new FlashWindow();

        // Wire up flash event
        if (_overlay.DataContext is OverlayViewModel vm)
        {
            vm.FillerDetected += (s, args) => _flash.Flash();
        }

        // Show both
        _flash.Show();
        _overlay.Show();
    }
}
