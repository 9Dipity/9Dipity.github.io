using ClinicBookingDemo.Core.Abstractions;
using ClinicBookingDemo.Core.Models;

namespace ClinicBookingDemo.Core.Services;

/// <summary>
/// Session-scoped translation service. Default language is Latvian, matching the target
/// audience (local Riga clinics); English is available via the in-app toggle.
/// </summary>
public class TranslationService : ITranslationService
{
    public Language CurrentLanguage { get; private set; } = Language.Lv;

    public event Action? LanguageChanged;

    public void SetLanguage(Language language)
    {
        if (CurrentLanguage == language)
        {
            return;
        }

        CurrentLanguage = language;
        LanguageChanged?.Invoke();
    }

    public string T(string key)
    {
        if (!Translations.Map.TryGetValue(key, out var entry))
        {
            return key;
        }

        return CurrentLanguage == Language.Lv ? entry.Lv : entry.En;
    }
}
