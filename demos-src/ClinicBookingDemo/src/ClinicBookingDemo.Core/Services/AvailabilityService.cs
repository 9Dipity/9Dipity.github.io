using ClinicBookingDemo.Core.Abstractions;
using ClinicBookingDemo.Core.Models;

namespace ClinicBookingDemo.Core.Services;

/// <summary>
/// Pure, testable availability logic. Depends only on <see cref="IClinicDataStore"/> — no
/// Blazor/browser dependency — so it can be exercised directly from xUnit.
/// </summary>
public class AvailabilityService : IAvailabilityService
{
    /// <summary>Granularity of candidate slot start times, independent of service duration.</summary>
    private const int SlotStepMinutes = 15;

    private readonly IClinicDataStore _store;

    public AvailabilityService(IClinicDataStore store)
    {
        _store = store;
    }

    public IReadOnlyList<TimeSlot> GetDaySlots(Guid specialistId, Guid serviceId, DateOnly date)
    {
        var specialist = _store.Specialists.FirstOrDefault(s => s.Id == specialistId);
        var service = _store.Services.FirstOrDefault(s => s.Id == serviceId);
        if (specialist is null || service is null)
        {
            return Array.Empty<TimeSlot>();
        }

        var hours = specialist.GetWorkingHours(date.DayOfWeek);
        if (hours is null)
        {
            return Array.Empty<TimeSlot>();
        }

        var duration = TimeSpan.FromMinutes(service.DurationMinutes);
        var dayStart = date.ToDateTime(hours.Value.Start);
        var dayEnd = date.ToDateTime(hours.Value.End);

        var slots = new List<TimeSlot>();
        for (var start = dayStart; start + duration <= dayEnd; start = start.AddMinutes(SlotStepMinutes))
        {
            var end = start + duration;
            var available = CanBook(specialistId, start, end);
            slots.Add(new TimeSlot(start, end, available));
        }

        return slots;
    }

    public IReadOnlyList<TimeSlot> GetAvailableSlots(Guid specialistId, Guid serviceId, DateOnly date) =>
        GetDaySlots(specialistId, serviceId, date).Where(s => s.IsAvailable).ToList();

    public bool CanBook(Guid specialistId, DateTime start, DateTime end)
    {
        if (end <= start)
        {
            return false;
        }

        var specialist = _store.Specialists.FirstOrDefault(s => s.Id == specialistId);
        if (specialist is null)
        {
            return false;
        }

        // Keep it simple: appointments never span midnight.
        if (start.Date != end.Date)
        {
            return false;
        }

        var hours = specialist.GetWorkingHours(start.DayOfWeek);
        if (hours is null)
        {
            return false;
        }

        var startTime = TimeOnly.FromDateTime(start);
        var endTime = TimeOnly.FromDateTime(end);
        if (startTime < hours.Value.Start || endTime > hours.Value.End)
        {
            return false;
        }

        var overlapsBooking = _store.Bookings.Any(b =>
            b.SpecialistId == specialistId &&
            b.Status != BookingStatus.Cancelled &&
            start < b.End && b.Start < end);
        if (overlapsBooking)
        {
            return false;
        }

        var overlapsBlock = _store.BlockedSlots.Any(b =>
            b.SpecialistId == specialistId &&
            start < b.End && b.Start < end);

        return !overlapsBlock;
    }
}
