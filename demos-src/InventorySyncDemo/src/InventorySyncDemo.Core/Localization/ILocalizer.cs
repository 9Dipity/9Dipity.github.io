namespace InventorySyncDemo.Core.Localization;

/// <summary>
/// Lightweight dictionary-based UI translation service. Holds the current language as
/// in-memory session state (register as a singleton) so switching language does not
/// reset dashboard/stock state - it only changes which strings <see cref="T"/> returns,
/// and components re-render via <see cref="LanguageChanged"/>.
/// </summary>
public interface ILocalizer
{
    Language CurrentLanguage { get; }

    event Action? LanguageChanged;

    void SetLanguage(Language language);

    /// <summary>Looks up the translated string for the given key in the current language.</summary>
    string T(string key);
}
