using System.Windows;
using UmBuster.Views;

namespace UmBuster;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Create and show the overlay window
        var overlay = new OverlayWindow();
        overlay.Show();
    }
}
