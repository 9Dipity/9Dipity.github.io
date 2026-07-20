namespace ClinicBookingDemo.Core.Models;

/// <summary>
/// The working window for a specialist on a given weekday. A specialist with no
/// entry (or a null value) for a given <see cref="DayOfWeek"/> does not work that day.
/// </summary>
/// <param name="Start">Start of the working day (local clinic time).</param>
/// <param name="End">End of the working day (local clinic time).</param>
public readonly record struct DayHours(TimeOnly Start, TimeOnly End);
