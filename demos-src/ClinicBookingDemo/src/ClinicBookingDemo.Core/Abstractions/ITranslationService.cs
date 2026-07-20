using ClinicBookingDemo.Core.Models;

namespace ClinicBookingDemo.Core.Abstractions;

/// <summary>
/// Lightweight dictionary-based translation service. Holds the current UI language for the
/// session and looks up translation keys. Default language is Latvian (local Riga clinics),
/// with English available via a toggle. No Blazor/browser dependency.
/// </summary>
public interface ITranslationService
{
    Language CurrentLanguage { get; }

    /// <summary>Raised when the language changes, so components can re-render in place.</summary>
    event Action? LanguageChanged;

    void SetLanguage(Language language);

    /// <summary>Translates <paramref name="key"/> for the current language. Returns the key itself if missing.</summary>
    string T(string key);
}
