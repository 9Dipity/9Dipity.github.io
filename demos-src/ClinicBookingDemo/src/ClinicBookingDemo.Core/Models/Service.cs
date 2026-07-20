namespace ClinicBookingDemo.Core.Models;

/// <summary>
/// A bookable appointment type, e.g. Checkup, Cleaning, Whitening, Emergency.
/// </summary>
public class Service
{
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>Translation key for the display name (e.g. "service.checkup.name").</summary>
    public string NameKey { get; init; } = string.Empty;

    /// <summary>Translation key for the short description.</summary>
    public string DescriptionKey { get; init; } = string.Empty;

    public int DurationMinutes { get; init; }
}
