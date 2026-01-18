using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace UmBuster.Services;

public partial class SettingsService : ObservableObject
{
    private readonly string _settingsPath;
    
    [ObservableProperty]
    private ObservableCollection<string> _fillerWords = new();

    public SettingsService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appData, "UmBuster");
        Directory.CreateDirectory(appFolder);
        _settingsPath = Path.Combine(appFolder, "settings.json");

        LoadSettings();
    }

    public void AddWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return;
        
        var trimmed = word.Trim().ToLower();
        if (!_fillerWords.Contains(trimmed))
        {
            _fillerWords.Add(trimmed);
            SaveSettings();
        }
    }

    public void RemoveWord(string word)
    {
        if (_fillerWords.Contains(word))
        {
            _fillerWords.Remove(word);
            SaveSettings();
        }
    }

    private void LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                var words = JsonSerializer.Deserialize<string[]>(json);
                if (words != null)
                {
                    FillerWords = new ObservableCollection<string>(words);
                    return;
                }
            }
        }
        catch 
        {
            // Fallback to defaults if load fails
        }

        // Defaults
        FillerWords = new ObservableCollection<string>
        {
            "um", "uh", "okay", "right", "so", "like", "you know", "ah", "er", "basically", "literally"
        };
    }

    private void SaveSettings()
    {
        try
        {
            var json = JsonSerializer.Serialize(_fillerWords);
            File.WriteAllText(_settingsPath, json);
        }
        catch
        {
            // Ignore save errors for now
        }
    }
}
