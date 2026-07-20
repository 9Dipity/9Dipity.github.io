using ClinicBookingDemo.Core.Models;

namespace ClinicBookingDemo.Core.Abstractions;

/// <summary>
/// Pure availability/conflict logic for a specialist: working hours, existing bookings,
/// and manually blocked time. Contains no UI/browser dependencies so it can be unit tested
/// directly against an injected <see cref="IClinicDataStore"/>.
/// </summary>
public interface IAvailabilityService
{
    /// <summary>
    /// Returns every candidate slot for the given specialist/service/day within working hours,
    /// each flagged as available or not (booked over, blocked, or otherwise unavailable), so the
    /// UI can render a full day grid with unavailable slots greyed out.
    /// </summary>
    IReadOnlyList<TimeSlot> GetDaySlots(Guid specialistId, Guid serviceId, DateOnly date);

    /// <summary>Returns only the bookable slots for the given specialist/service/day.</summary>
    IReadOnlyList<TimeSlot> GetAvailableSlots(Guid specialistId, Guid serviceId, DateOnly date);

    /// <summary>Whether the given specialist could be booked for exactly this start/end window.</summary>
    bool CanBook(Guid specialistId, DateTime start, DateTime end);
}
