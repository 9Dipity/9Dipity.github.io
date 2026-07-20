using ClinicBookingDemo.Core.Models;

namespace ClinicBookingDemo.Core.Abstractions;

/// <summary>
/// In-memory storage for all clinic data (specialists, services, bookings, blocked slots).
/// Seeded on startup with fake demo data. Not thread-safe beyond what a single-user WASM
/// session requires.
/// </summary>
public interface IClinicDataStore
{
    IReadOnlyList<Specialist> Specialists { get; }

    IReadOnlyList<Service> Services { get; }

    IReadOnlyList<Booking> Bookings { get; }

    IReadOnlyList<BlockedSlot> BlockedSlots { get; }

    /// <summary>Raised whenever bookings, blocked slots, or booking statuses change, so the UI can re-render.</summary>
    event Action? Changed;

    Booking AddBooking(Booking booking);

    void UpdateBookingStatus(Guid bookingId, BookingStatus status);

    BlockedSlot AddBlockedSlot(BlockedSlot slot);

    void RemoveBlockedSlot(Guid blockedSlotId);

    /// <summary>Restores the original seed data, discarding any bookings/blocks made during the session.</summary>
    void Reset();
}
