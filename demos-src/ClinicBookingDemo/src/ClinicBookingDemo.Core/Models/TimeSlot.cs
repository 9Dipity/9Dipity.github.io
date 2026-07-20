namespace ClinicBookingDemo.Core.Models;

/// <summary>
/// A candidate appointment window on the public booking calendar, with whether it can
/// actually be booked (used by the UI to grey out unavailable slots).
/// </summary>
public readonly record struct TimeSlot(DateTime Start, DateTime End, bool IsAvailable);
