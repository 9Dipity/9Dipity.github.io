namespace ClinicBookingDemo.Core.Models;

public class Booking
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid SpecialistId { get; init; }

    public Guid ServiceId { get; init; }

    public string PatientName { get; init; } = string.Empty;

    public string PatientPhone { get; init; } = string.Empty;

    public string PatientEmail { get; init; } = string.Empty;

    public DateTime Start { get; init; }

    public DateTime End { get; init; }

    public BookingStatus Status { get; set; } = BookingStatus.Booked;
}
