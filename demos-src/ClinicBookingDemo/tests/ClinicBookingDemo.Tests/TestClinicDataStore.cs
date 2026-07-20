using ClinicBookingDemo.Core.Abstractions;
using ClinicBookingDemo.Core.Models;

namespace ClinicBookingDemo.Tests;

/// <summary>
/// Minimal, unseeded <see cref="IClinicDataStore"/> for tests: the caller sets up exactly the
/// specialists/services/bookings/blocks a given test needs, with no demo seed data in the way.
/// </summary>
public class TestClinicDataStore : IClinicDataStore
{
    private readonly List<Specialist> _specialists = new();
    private readonly List<Service> _services = new();
    private readonly List<Booking> _bookings = new();
    private readonly List<BlockedSlot> _blockedSlots = new();

    public IReadOnlyList<Specialist> Specialists => _specialists;
    public IReadOnlyList<Service> Services => _services;
    public IReadOnlyList<Booking> Bookings => _bookings;
    public IReadOnlyList<BlockedSlot> BlockedSlots => _blockedSlots;

    public event Action? Changed;

    public void AddSpecialist(Specialist specialist) => _specialists.Add(specialist);

    public void AddService(Service service) => _services.Add(service);

    public Booking AddBooking(Booking booking)
    {
        _bookings.Add(booking);
        Changed?.Invoke();
        return booking;
    }

    public void UpdateBookingStatus(Guid bookingId, BookingStatus status)
    {
        var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
        if (booking is null) return;
        booking.Status = status;
        Changed?.Invoke();
    }

    public BlockedSlot AddBlockedSlot(BlockedSlot slot)
    {
        _blockedSlots.Add(slot);
        Changed?.Invoke();
        return slot;
    }

    public void RemoveBlockedSlot(Guid blockedSlotId)
    {
        _blockedSlots.RemoveAll(b => b.Id == blockedSlotId);
        Changed?.Invoke();
    }

    public void Reset()
    {
        _bookings.Clear();
        _blockedSlots.Clear();
        Changed?.Invoke();
    }
}
