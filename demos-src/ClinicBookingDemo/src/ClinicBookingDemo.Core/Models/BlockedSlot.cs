namespace ClinicBookingDemo.Core.Models;

/// <summary>
/// A manually blocked window of time for a specialist (e.g. on leave, lunch, training)
/// during which no bookings may be made, even though it falls within working hours.
/// </summary>
public class BlockedSlot
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid SpecialistId { get; init; }

    public DateTime Start { get; init; }

    public DateTime End { get; init; }

    /// <summary>Translation key for the reason shown in the admin view, e.g. "blocked.onLeave".</summary>
    public string ReasonKey { get; init; } = string.Empty;
}
