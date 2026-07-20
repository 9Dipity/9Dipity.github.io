namespace ClinicBookingDemo.Core.Models;

/// <summary>
/// A clinic staff member (dentist/hygienist) who can be booked for appointments.
/// </summary>
public class Specialist
{
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>Fictional proper name — not translated.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Translation key for the specialty title, e.g. "specialist.title.generalDentist".</summary>
    public string TitleKey { get; init; } = string.Empty;

    /// <summary>Translation key for the short bio line shown on the specialist picker card.</summary>
    public string BioKey { get; init; } = string.Empty;

    /// <summary>Hex color used to color-code this specialist's bookings in the admin calendar.</summary>
    public string Color { get; init; } = "#2E7D6B";

    /// <summary>Working hours per weekday. A missing key or null value means the specialist does not work that day.</summary>
    public IReadOnlyDictionary<DayOfWeek, DayHours?> WorkingHours { get; init; } =
        new Dictionary<DayOfWeek, DayHours?>();

    public DayHours? GetWorkingHours(DayOfWeek day) =>
        WorkingHours.TryGetValue(day, out var hours) ? hours : null;
}
