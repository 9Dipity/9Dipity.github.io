using Microsoft.AspNetCore.Components;
using DbHealthCheckDemo.Core.Localization;

namespace DbHealthCheckDemo.Client.Shared;

/// <summary>
/// Base class for any component whose markup calls Localizer.T(...). Subscribes to
/// ILocalizer.LanguageChanged so flipping the EN/LV toggle re-renders the component in
/// place, without touching any audit state.
/// </summary>
public abstract class LocalizedComponentBase : ComponentBase, IDisposable
{
    [Inject]
    protected ILocalizer Localizer { get; set; } = default!;

    protected override void OnInitialized()
    {
        Localizer.LanguageChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    public virtual void Dispose()
    {
        Localizer.LanguageChanged -= OnLanguageChanged;
    }
}
