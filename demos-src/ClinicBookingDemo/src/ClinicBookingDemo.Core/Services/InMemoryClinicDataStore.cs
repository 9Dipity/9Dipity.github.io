using ClinicBookingDemo.Core.Abstractions;
using ClinicBookingDemo.Core.Models;

namespace ClinicBookingDemo.Core.Services;

/// <summary>
/// In-memory implementation of <see cref="IClinicDataStore"/>, seeded with fake demo data on
/// construction. Registered as a singleton for the lifetime of the WASM session so the whole
/// app shares one set of data.
/// </summary>
public class InMemoryClinicDataStore : IClinicDataStore
{
    private List<Specialist> _specialists = new();
    private List<Service> _services = new();
    private List<Booking> _bookings = new();
    private List<BlockedSlot> _blockedSlots = new();

    public InMemoryClinicDataStore()
    {
        Seed();
    }

    public IReadOnlyList<Specialist> Specialists => _specialists;

    public IReadOnlyList<Service> Services => _services;

    public IReadOnlyList<Booking> Bookings => _bookings;

    public IReadOnlyList<BlockedSlot> BlockedSlots => _blockedSlots;

    public event Action? Changed;

    public Booking AddBooking(Booking booking)
    {
        _bookings.Add(booking);
        Changed?.Invoke();
        return booking;
    }

    public void UpdateBookingStatus(Guid bookingId, BookingStatus status)
    {
        var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
        if (booking is null)
        {
            return;
        }

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
        Seed();
        Changed?.Invoke();
    }

    private void Seed()
    {
        _specialists = ClinicSeedData.CreateSpecialists();
        _services = ClinicSeedData.CreateServices();
        var today = DateOnly.FromDateTime(DateTime.Now);
        _bookings = ClinicSeedData.CreateBookings(_specialists, _services, today);
        _blockedSlots = ClinicSeedData.CreateBlockedSlots(_specialists, today);
    }
}
