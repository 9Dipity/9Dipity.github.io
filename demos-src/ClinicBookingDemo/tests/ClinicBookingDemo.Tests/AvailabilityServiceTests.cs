using ClinicBookingDemo.Core.Models;
using ClinicBookingDemo.Core.Services;
using Xunit;

namespace ClinicBookingDemo.Tests;

public class AvailabilityServiceTests
{
    // Monday, Jan 8 2024 — fixed so tests never depend on "today".
    private static readonly DateOnly Monday = new(2024, 1, 8);

    private static readonly Guid SpecialistAId = Guid.NewGuid();
    private static readonly Guid SpecialistBId = Guid.NewGuid();
    private static readonly Guid ServiceId = Guid.NewGuid();

    private static (TestClinicDataStore Store, AvailabilityService Sut) BuildSut()
    {
        var store = new TestClinicDataStore();

        store.AddSpecialist(new Specialist
        {
            Id = SpecialistAId,
            Name = "Dr. Test A",
            TitleKey = "specialist.title.generalDentist",
            WorkingHours = new Dictionary<DayOfWeek, DayHours?>
            {
                [DayOfWeek.Monday] = new DayHours(new TimeOnly(9, 0), new TimeOnly(17, 0)),
            }
        });

        store.AddSpecialist(new Specialist
        {
            Id = SpecialistBId,
            Name = "Dr. Test B",
            TitleKey = "specialist.title.orthodontist",
            WorkingHours = new Dictionary<DayOfWeek, DayHours?>
            {
                [DayOfWeek.Monday] = new DayHours(new TimeOnly(9, 0), new TimeOnly(17, 0)),
            }
        });

        store.AddService(new Service { Id = ServiceId, NameKey = "service.checkup.name", DescriptionKey = "service.checkup.desc", DurationMinutes = 30 });

        var sut = new AvailabilityService(store);
        return (store, sut);
    }

    [Fact]
    public void CanBook_ReturnsTrue_ForOpenSlotWithinWorkingHours()
    {
        var (_, sut) = BuildSut();
        var start = Monday.ToDateTime(new TimeOnly(10, 0));
        var end = start.AddMinutes(30);

        Assert.True(sut.CanBook(SpecialistAId, start, end));
    }

    [Fact]
    public void CanBook_RejectsExactOverlap_SameSpecialist()
    {
        var (store, sut) = BuildSut();
        var start = Monday.ToDateTime(new TimeOnly(10, 0));
        var end = start.AddMinutes(30);

        store.AddBooking(new Booking
        {
            SpecialistId = SpecialistAId,
            ServiceId = ServiceId,
            Start = start,
            End = end,
            Status = BookingStatus.Booked
        });

        Assert.False(sut.CanBook(SpecialistAId, start, end));
    }

    [Fact]
    public void CanBook_RejectsPartialOverlap_SameSpecialist()
    {
        var (store, sut) = BuildSut();
        var existingStart = Monday.ToDateTime(new TimeOnly(10, 0));
        var existingEnd = existingStart.AddMinutes(30);

        store.AddBooking(new Booking
        {
            SpecialistId = SpecialistAId,
            ServiceId = ServiceId,
            Start = existingStart,
            End = existingEnd,
            Status = BookingStatus.Booked
        });

        // Overlaps the last 15 minutes of the existing booking and runs 15 minutes past it.
        var newStart = Monday.ToDateTime(new TimeOnly(10, 15));
        var newEnd = newStart.AddMinutes(30);

        Assert.False(sut.CanBook(SpecialistAId, newStart, newEnd));
    }

    [Fact]
    public void CanBook_RejectsSlotOutsideWorkingHours()
    {
        var (_, sut) = BuildSut();

        // Specialist A works 09:00-17:00 on Monday; 17:30 start is outside that window.
        var start = Monday.ToDateTime(new TimeOnly(17, 30));
        var end = start.AddMinutes(30);

        Assert.False(sut.CanBook(SpecialistAId, start, end));
    }

    [Fact]
    public void CanBook_RejectsSlotOnADayTheSpecialistDoesNotWork()
    {
        var (_, sut) = BuildSut();

        var tuesday = Monday.AddDays(1);
        var start = tuesday.ToDateTime(new TimeOnly(10, 0));
        var end = start.AddMinutes(30);

        Assert.False(sut.CanBook(SpecialistAId, start, end));
    }

    [Fact]
    public void CanBook_RejectsManuallyBlockedSlot()
    {
        var (store, sut) = BuildSut();
        var start = Monday.ToDateTime(new TimeOnly(13, 0));
        var end = start.AddMinutes(30);

        store.AddBlockedSlot(new BlockedSlot
        {
            SpecialistId = SpecialistAId,
            Start = Monday.ToDateTime(new TimeOnly(12, 0)),
            End = Monday.ToDateTime(new TimeOnly(14, 0)),
            ReasonKey = "blocked.onLeave"
        });

        Assert.False(sut.CanBook(SpecialistAId, start, end));
    }

    [Fact]
    public void GetAvailableSlots_DoesNotIncludeBlockedWindow()
    {
        var (store, sut) = BuildSut();

        store.AddBlockedSlot(new BlockedSlot
        {
            SpecialistId = SpecialistAId,
            Start = Monday.ToDateTime(new TimeOnly(12, 0)),
            End = Monday.ToDateTime(new TimeOnly(14, 0)),
            ReasonKey = "blocked.onLeave"
        });

        var slots = sut.GetAvailableSlots(SpecialistAId, ServiceId, Monday);

        Assert.DoesNotContain(slots, s => s.Start < Monday.ToDateTime(new TimeOnly(14, 0)) && s.End > Monday.ToDateTime(new TimeOnly(12, 0)));
        Assert.NotEmpty(slots);
    }

    [Fact]
    public void CanBook_SucceedsForDifferentSpecialist_AtSameTime()
    {
        var (store, sut) = BuildSut();
        var start = Monday.ToDateTime(new TimeOnly(10, 0));
        var end = start.AddMinutes(30);

        store.AddBooking(new Booking
        {
            SpecialistId = SpecialistAId,
            ServiceId = ServiceId,
            Start = start,
            End = end,
            Status = BookingStatus.Booked
        });

        // Same exact time window, but a different specialist — should not conflict.
        Assert.True(sut.CanBook(SpecialistBId, start, end));
    }

    [Fact]
    public void CanBook_SucceedsForNonOverlappingBackToBackBookings_SameSpecialist()
    {
        var (store, sut) = BuildSut();
        var firstStart = Monday.ToDateTime(new TimeOnly(10, 0));
        var firstEnd = firstStart.AddMinutes(30);

        Assert.True(sut.CanBook(SpecialistAId, firstStart, firstEnd));
        store.AddBooking(new Booking
        {
            SpecialistId = SpecialistAId,
            ServiceId = ServiceId,
            Start = firstStart,
            End = firstEnd,
            Status = BookingStatus.Booked
        });

        // Starts exactly when the first one ends — back-to-back, no overlap.
        var secondStart = firstEnd;
        var secondEnd = secondStart.AddMinutes(30);

        Assert.True(sut.CanBook(SpecialistAId, secondStart, secondEnd));
    }

    [Fact]
    public void CanBook_IgnoresCancelledBookings_WhenCheckingOverlap()
    {
        var (store, sut) = BuildSut();
        var start = Monday.ToDateTime(new TimeOnly(10, 0));
        var end = start.AddMinutes(30);

        store.AddBooking(new Booking
        {
            SpecialistId = SpecialistAId,
            ServiceId = ServiceId,
            Start = start,
            End = end,
            Status = BookingStatus.Cancelled
        });

        Assert.True(sut.CanBook(SpecialistAId, start, end));
    }
}
