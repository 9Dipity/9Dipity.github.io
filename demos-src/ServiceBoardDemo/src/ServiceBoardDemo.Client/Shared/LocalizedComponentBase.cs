using Microsoft.AspNetCore.Components;
using ServiceBoardDemo.Core.Localization;

namespace ServiceBoardDemo.Client.Shared;

/// <summary>
/// Base class for any component whose markup calls Localizer.T(...). Subscribes to
/// ILocalizer.LanguageChanged so flipping the EN/LV toggle re-renders the component in
/// place, without touching any job-board state.
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
