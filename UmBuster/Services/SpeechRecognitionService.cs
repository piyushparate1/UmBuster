using System;
using System.IO;
using Vosk;

namespace UmBuster.Services;

public class SpeechRecognitionService : IDisposable
{
    private Model? _model;
    private VoskRecognizer? _recognizer;
    public event EventHandler<string>? TextRecognized;

    public bool IsInitialized => _model != null;

    public void Initialize(string modelPath)
    {
        if (!Directory.Exists(modelPath))
        {
            throw new DirectoryNotFoundException($"Vosk model not found at: {modelPath}");
        }

        // Vosk configuration logs can be noisy, you might want to suppress them
        Vosk.Vosk.SetLogLevel(-1);

        _model = new Model(modelPath);
        _recognizer = new VoskRecognizer(_model, 16000.0f);
    }

    public void ProcessAudio(byte[] buffer, int bytesRecorded)
    {
        if (_recognizer == null) return;

        if (_recognizer.AcceptWaveform(buffer, bytesRecorded))
        {
            // Full result
            var result = _recognizer.Result();
            ExtractAndRaiseText(result);
        }
        else
        {
            // Partial result
            // var partial = _recognizer.PartialResult();
            // We can use partial results for faster feedback if needed, 
            // but for filler words, strict "Result" might be safer to avoid false positives during stuttering.
            // Let's stick to "Result" for now or parse partial if we need ultra-low latency.
            // For now, let's just stick to the finalized results or handle partial updates if we parsing logic allows.
            
            // Actually, for "um", "uh", they are short. Partial results update frequently. 
            // Let's try to expose partials too if needed, but for simplicity let's start with finalized results.
            // Wait, "um" might be a whole sentence like "Um..." so it will be a result.
        }
    }
    
    // Vosk returns JSON: { "text": "..." }
    private void ExtractAndRaiseText(string jsonResult)
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(jsonResult);
            if (doc.RootElement.TryGetProperty("text", out var textProp))
            {
                var text = textProp.GetString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    TextRecognized?.Invoke(this, text);
                }
            }
        }
        catch { /* Ignore parse errors */ }
    }

    public void Dispose()
    {
        _recognizer?.Dispose();
        _model?.Dispose();
        GC.SuppressFinalize(this);
    }
}
