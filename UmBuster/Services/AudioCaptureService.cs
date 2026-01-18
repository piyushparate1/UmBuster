using NAudio.Wave;
using System;

namespace UmBuster.Services;

public class AudioCaptureService : IDisposable
{
    private readonly WaveInEvent _waveIn;
    public event EventHandler<WaveInEventArgs>? DataAvailable;

    public AudioCaptureService()
    {
        _waveIn = new WaveInEvent
        {
            WaveFormat = new WaveFormat(16000, 1), // Vosk requires 16kHz mono
            BufferMilliseconds = 20
        };

        _waveIn.DataAvailable += (s, e) => DataAvailable?.Invoke(this, e);
    }

    public void Start()
    {
        try 
        {
            _waveIn.StartRecording();
        }
        catch (Exception ex)
        {
            // Ideally log this
            System.Diagnostics.Debug.WriteLine($"Error starting audio capture: {ex.Message}");
            throw;
        }
    }

    public void Stop()
    {
        _waveIn.StopRecording();
    }

    public void Dispose()
    {
        _waveIn?.Dispose();
        GC.SuppressFinalize(this);
    }
}
