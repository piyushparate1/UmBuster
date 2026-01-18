using System;
using System.Linq;
using UmBuster.Models;

namespace UmBuster.Services;

public class FillerWordDetector
{
    private readonly SettingsService _settingsService;
    public event EventHandler<FillerWordEvent>? FillerWordDetected;

    public FillerWordDetector(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public void Analyze(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        // Simple tokenization
        // Vosk output is usually lowercase and without punctuation, but let's be safe
        var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            var cleanedWord = word.Trim().ToLower();
            
            // Check exact matches against settings
            // This is O(N*M) but N (words in sentence) and M (filler words) are very small.
            if (_settingsService.FillerWords.Contains(cleanedWord))
            {
                FillerWordDetected?.Invoke(this, new FillerWordEvent(cleanedWord, DateTime.Now));
            }
        }
        
        // Advanced: Multi-word fillers like "you know"
        // This simple loop misses them. Let's add a quick check for phrases if needed.
        // Doing a quick pass for the multi-word phrases in our list.
        foreach (var phrase in _settingsService.FillerWords.Where(w => w.Contains(" ")))
        {
            if (text.ToLower().Contains(phrase))
            {
                // Simple containment check. 
                // Note: "you know" in "do you know" is not a filler. Context matters but is hard.
                // For now, let's stick to this simple approach or refine it.
                // User requirement: "simple and minimal" -> let's stick to simple detection.
                
                // To avoid double counting (e.g. "you" and "know" detected separately if they are in the list + "you know"), 
                // we'd need more complex logic. 
                // For this MVP, let's assume single words are primary, and "you know" is a special case.
                
                FillerWordDetected?.Invoke(this, new FillerWordEvent(phrase, DateTime.Now));
            }
        }
    }
}
