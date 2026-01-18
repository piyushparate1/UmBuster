using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using UmBuster.Services;

namespace UmBuster.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;

    [ObservableProperty]
    private string _newWord = string.Empty;

    public ObservableCollection<string> FillerWords => _settingsService.FillerWords;

    public SettingsViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [RelayCommand]
    private void AddWord()
    {
        if (!string.IsNullOrWhiteSpace(NewWord))
        {
            _settingsService.AddWord(NewWord);
            NewWord = string.Empty;
        }
    }

    [RelayCommand]
    private void RemoveWord(string word)
    {
        _settingsService.RemoveWord(word);
    }
}
