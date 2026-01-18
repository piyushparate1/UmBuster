using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using UmBuster.Services;

namespace UmBuster.ViewModels;

public partial class OverlayViewModel : ObservableObject, IDisposable
{
    private readonly AudioCaptureService _audioService;
    private readonly SpeechRecognitionService _speechService;
    private readonly FillerWordDetector _detector;
    private readonly SettingsService _settingsService;

    [ObservableProperty]
    private int _fillerCount;

    [ObservableProperty]
    private string _lastFillerWord = "--";

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private string _statusMessage = "Initializing...";

    public OverlayViewModel()
    {
        // simplistic DI for this scale
        _settingsService = new SettingsService();
        _audioService = new AudioCaptureService();
        _speechService = new SpeechRecognitionService();
        _detector = new FillerWordDetector(_settingsService);

        _audioService.DataAvailable += OnAudioDataAvailable;
        _speechService.TextRecognized += OnTextRecognized;
        _detector.FillerWordDetected += OnFillerDetected;
    }

    public void Initialize()
    {
        // Initialize model in background to avoid freezing UI
        System.Threading.Tasks.Task.Run(async () => 
        {
            try 
            {
                var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UmBuster", "Models");
                Directory.CreateDirectory(appData);

                var modelName = "vosk-model-small-en-us-0.15";
                var modelPath = Path.Combine(appData, modelName);

                if (!Directory.Exists(modelPath))
                {
                    Application.Current.Dispatcher.Invoke(() => StatusMessage = "Downloading Model...");
                    var zipPath = Path.Combine(appData, modelName + ".zip");
                    var url = "https://alphacephei.com/vosk/models/vosk-model-small-en-us-0.15.zip";

                    using (var client = new System.Net.Http.HttpClient())
                    {
                        var bytes = await client.GetByteArrayAsync(url);
                        await File.WriteAllBytesAsync(zipPath, bytes);
                    }

                    Application.Current.Dispatcher.Invoke(() => StatusMessage = "Extracting...");
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, appData);
                    File.Delete(zipPath);
                }

                Application.Current.Dispatcher.Invoke(() => StatusMessage = "Loading Engine...");
                _speechService.Initialize(modelPath);
                
                Application.Current.Dispatcher.Invoke(() => 
                {
                    StatusMessage = "Ready";
                    Start();
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => StatusMessage = "Model Error");
                Debug.WriteLine($"Init error: {ex}");
            }
        });
    }

    [RelayCommand]
    private void ToggleWrapper()
    {
        if (IsRunning) Stop();
        else Start();
    }

    private void Start()
    {
        if (!_speechService.IsInitialized) return;
        try 
        {
            _audioService.Start();
            IsRunning = true;
            StatusMessage = "Listening";
        }
        catch 
        {
            StatusMessage = "Mic Error";
        }
    }

    private void Stop()
    {
        _audioService.Stop();
        IsRunning = false;
        StatusMessage = "Paused";
    }

    [RelayCommand]
    private void OpenSettings()
    {
        // We'll handle window opening in View code-behind or a service, 
        // but for simplicity let's rely on the View code-behind subscribing to an event 
        // or passing the VM to a new window.
        // Actually, let's expose an event here that the View subscribes to.
        OpenSettingsRequested?.Invoke(this, EventArgs.Empty);
    }
    
    public event EventHandler? OpenSettingsRequested;
    
    public SettingsViewModel CreateSettingsViewModel()
    {
        return new SettingsViewModel(_settingsService);
    }

    private void OnAudioDataAvailable(object? sender, NAudio.Wave.WaveInEventArgs e)
    {
        if (IsRunning)
        {
            _speechService.ProcessAudio(e.Buffer, e.BytesRecorded);
        }
    }

    private void OnTextRecognized(object? sender, string text)
    {
        _detector.Analyze(text);
        Debug.WriteLine($"Recognized: {text}");
    }

    public event EventHandler? FillerDetected; // Added public event

    private void OnFillerDetected(object? sender, Models.FillerWordEvent e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            FillerCount++;
            LastFillerWord = e.Word;
            FillerDetected?.Invoke(this, EventArgs.Empty); // Raise event
            
            // Optional: Reset last word after a few seconds?
        });
    }

    public void Dispose()
    {
        Stop();
        _audioService.Dispose();
        _speechService.Dispose();
    }
}
