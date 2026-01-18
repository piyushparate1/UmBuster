using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace UmBuster.Views;

public partial class FlashWindow : Window
{
    private Storyboard? _flashStoryboard;

    public FlashWindow()
    {
        InitializeComponent();
        Loaded += (s, e) => 
        {
            _flashStoryboard = FindResource("FlashAnimation") as Storyboard;
        };
    }

    public void Flash()
    {
        Dispatcher.Invoke(() => 
        {
            _flashStoryboard?.Begin();
        });
    }
}
