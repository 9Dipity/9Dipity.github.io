namespace DbHealthCheckDemo.Core.Localization;

public interface ILocalizer
{
    Language CurrentLanguage { get; }
    event Action? LanguageChanged;
    void SetLanguage(Language language);
    string T(string key);
}
