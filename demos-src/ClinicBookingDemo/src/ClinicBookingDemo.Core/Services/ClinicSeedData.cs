using ClinicBookingDemo.Core.Models;

namespace ClinicBookingDemo.Core.Services;

/// <summary>
/// Generates the fake demo data for Rigas Smaids Clinic: specialists, services, and a spread
/// of bookings across the past two weeks and the next two weeks (recomputed relative to
/// "today" every time it is called, so the demo never looks stale). Entirely fictional.
/// </summary>
public static class ClinicSeedData
{
    public static readonly Guid DrKalninaId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid DrOzolsId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid DrBerzinaId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static readonly Guid CheckupId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");
    public static readonly Guid CleaningId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002");
    public static readonly Guid WhiteningId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003");
    public static readonly Guid EmergencyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004");

    public static List<Specialist> CreateSpecialists() => new()
    {
        new Specialist
        {
            Id = DrKalninaId,
            Name = "Dr. Elīna Kalniņa",
            TitleKey = "specialist.title.generalDentist",
            BioKey = "specialist.bio.kalnina",
            Color = "#2E7D6B",
            WorkingHours = new Dictionary<DayOfWeek, DayHours?>
            {
                [DayOfWeek.Monday] = new DayHours(new TimeOnly(9, 0), new TimeOnly(17, 0)),
                [DayOfWeek.Tuesday] = new DayHours(new TimeOnly(9, 0), new TimeOnly(17, 0)),
                [DayOfWeek.Wednesday] = new DayHours(new TimeOnly(9, 0), new TimeOnly(17, 0)),
                [DayOfWeek.Thursday] = new DayHours(new TimeOnly(9, 0), new TimeOnly(17, 0)),
                [DayOfWeek.Friday] = new DayHours(new TimeOnly(9, 0), new TimeOnly(17, 0)),
            }
        },
        new Specialist
        {
            Id = DrOzolsId,
            Name = "Dr. Mārtiņš Ozols",
            TitleKey = "specialist.title.orthodontist",
            BioKey = "specialist.bio.ozols",
            Color = "#3B6FA0",
            WorkingHours = new Dictionary<DayOfWeek, DayHours?>
            {
                [DayOfWeek.Tuesday] = new DayHours(new TimeOnly(10, 0), new TimeOnly(18, 0)),
                [DayOfWeek.Thursday] = new DayHours(new TimeOnly(10, 0), new TimeOnly(18, 0)),
                [DayOfWeek.Friday] = new DayHours(new TimeOnly(10, 0), new TimeOnly(15, 0)),
            }
        },
        new Specialist
        {
            Id = DrBerzinaId,
            Name = "Dr. Zane Bērziņa",
            TitleKey = "specialist.title.hygienist",
            BioKey = "specialist.bio.berzina",
            Color = "#C9743B",
            WorkingHours = new Dictionary<DayOfWeek, DayHours?>
            {
                [DayOfWeek.Monday] = new DayHours(new TimeOnly(8, 0), new TimeOnly(14, 0)),
                [DayOfWeek.Wednesday] = new DayHours(new TimeOnly(8, 0), new TimeOnly(14, 0)),
                [DayOfWeek.Friday] = new DayHours(new TimeOnly(8, 0), new TimeOnly(14, 0)),
                [DayOfWeek.Saturday] = new DayHours(new TimeOnly(9, 0), new TimeOnly(13, 0)),
            }
        }
    };

    public static List<Service> CreateServices() => new()
    {
        new Service { Id = CheckupId, NameKey = "service.checkup.name", DescriptionKey = "service.checkup.desc", DurationMinutes = 30 },
        new Service { Id = CleaningId, NameKey = "service.cleaning.name", DescriptionKey = "service.cleaning.desc", DurationMinutes = 45 },
        new Service { Id = WhiteningId, NameKey = "service.whitening.name", DescriptionKey = "service.whitening.desc", DurationMinutes = 60 },
        new Service { Id = EmergencyId, NameKey = "service.emergency.name", DescriptionKey = "service.emergency.desc", DurationMinutes = 30 },
    };

    private static readonly (string Name, string Phone, string Email)[] Patients =
    {
        ("Anna Kļaviņa", "+371 2011 2233", "anna.klavina@example.com"),
        ("Jānis Liepa", "+371 2022 3344", "janis.liepa@example.com"),
        ("Laura Ozoliņa", "+371 2033 4455", "laura.ozolina@example.com"),
        ("Kristaps Circenis", "+371 2044 5566", "kristaps.circenis@example.com"),
        ("Ilze Vītola", "+371 2055 6677", "ilze.vitola@example.com"),
        ("Toms Bērziņš", "+371 2066 7788", "toms.berzins@example.com"),
        ("Signe Zariņa", "+371 2077 8899", "signe.zarina@example.com"),
        ("Reinis Kalējs", "+371 2088 9900", "reinis.kalejs@example.com"),
        ("Agnese Priede", "+371 2099 0011", "agnese.priede@example.com"),
        ("Edgars Students", "+371 2100 1122", "edgars.students@example.com"),
        ("Baiba Krūmiņa", "+371 2111 2233", "baiba.kruminа@example.com"),
        ("Uldis Ābele", "+371 2122 3344", "uldis.abele@example.com"),
        ("Marta Dūmiņa", "+371 2133 4455", "marta.dumina@example.com"),
        ("Rihards Sniedze", "+371 2144 5566", "rihards.sniedze@example.com"),
        ("Gunta Lapsa", "+371 2155 6677", "gunta.lapsa@example.com"),
    };

    /// <summary>
    /// Builds a spread of bookings across roughly the past two weeks and the next two weeks,
    /// anchored to <paramref name="today"/>. Deterministic (fixed RNG seed) so the demo looks
    /// the same on every reset, but the actual calendar dates always sit around "now".
    /// </summary>
    public static List<Booking> CreateBookings(IReadOnlyList<Specialist> specialists, IReadOnlyList<Service> services, DateOnly today)
    {
        var rng = new Random(20240501);
        var bookings = new List<Booking>();
        var occupied = new Dictionary<Guid, List<(DateTime Start, DateTime End)>>();

        bool Overlaps(Guid specialistId, DateTime start, DateTime end)
        {
            if (!occupied.TryGetValue(specialistId, out var list))
            {
                return false;
            }
            return list.Any(o => start < o.End && o.Start < end);
        }

        void Reserve(Guid specialistId, DateTime start, DateTime end)
        {
            if (!occupied.TryGetValue(specialistId, out var list))
            {
                list = new List<(DateTime, DateTime)>();
                occupied[specialistId] = list;
            }
            list.Add((start, end));
        }

        int patientCursor = 0;
        (string Name, string Phone, string Email) NextPatient()
        {
            var p = Patients[patientCursor % Patients.Length];
            patientCursor++;
            return p;
        }

        bool TryPlaceBooking(Specialist specialist, DateOnly date, DayHours hours)
        {
            var service = services[rng.Next(services.Count)];
            var duration = TimeSpan.FromMinutes(service.DurationMinutes);

            var dayStart = date.ToDateTime(hours.Start);
            var dayEnd = date.ToDateTime(hours.End);

            // Try a handful of random slot starts (15-min grid) until we find one that
            // doesn't collide with what we've already placed for this specialist that day.
            for (var attempt = 0; attempt < 12; attempt++)
            {
                var maxSteps = (int)((dayEnd - dayStart - duration).TotalMinutes / 15);
                if (maxSteps < 0)
                {
                    return false;
                }
                var step = rng.Next(0, maxSteps + 1);
                var start = dayStart.AddMinutes(step * 15);
                var end = start + duration;

                if (Overlaps(specialist.Id, start, end))
                {
                    continue;
                }

                Reserve(specialist.Id, start, end);

                var status = BookingStatus.Booked;
                if (start < DateTime.Now)
                {
                    // Past appointments are mostly completed, a few no-shows.
                    status = rng.NextDouble() < 0.85 ? BookingStatus.Completed : BookingStatus.NoShow;
                }

                var patient = NextPatient();
                bookings.Add(new Booking
                {
                    SpecialistId = specialist.Id,
                    ServiceId = service.Id,
                    PatientName = patient.Name,
                    PatientPhone = patient.Phone,
                    PatientEmail = patient.Email,
                    Start = start,
                    End = end,
                    Status = status
                });
                return true;
            }

            return false;
        }

        // Every (specialist, working day) combination in the -12..+13 day window is a candidate
        // for one booking. We guarantee "today" is covered (for the Today's Schedule wow-moment)
        // and then fill the rest of a ~15-20 booking target randomly across the whole window, so
        // the calendar looks like a realistically-busy clinic rather than fully packed.
        const int TargetBookingCount = 18;

        var todayCandidates = new List<(Specialist Specialist, DateOnly Date, DayHours Hours)>();
        var otherCandidates = new List<(Specialist Specialist, DateOnly Date, DayHours Hours)>();

        for (var offset = -12; offset <= 13; offset++)
        {
            var date = today.AddDays(offset);
            foreach (var specialist in specialists)
            {
                var hours = specialist.GetWorkingHours(date.DayOfWeek);
                if (hours is null)
                {
                    continue;
                }

                if (offset == 0)
                {
                    todayCandidates.Add((specialist, date, hours.Value));
                }
                else
                {
                    otherCandidates.Add((specialist, date, hours.Value));
                }
            }
        }

        // Guarantee at least one booking today for every specialist who works today.
        foreach (var (specialist, date, hours) in todayCandidates)
        {
            TryPlaceBooking(specialist, date, hours);
        }

        // Shuffle (Fisher-Yates) the remaining candidates and fill up to the target count.
        for (var i = otherCandidates.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (otherCandidates[i], otherCandidates[j]) = (otherCandidates[j], otherCandidates[i]);
        }

        foreach (var (specialist, date, hours) in otherCandidates)
        {
            if (bookings.Count >= TargetBookingCount)
            {
                break;
            }
            TryPlaceBooking(specialist, date, hours);
        }

        return bookings;
    }

    /// <summary>
    /// At least one manually blocked slot so the admin demo has something to show off out of the box:
    /// an upcoming afternoon where a specialist is "on leave".
    /// </summary>
    public static List<BlockedSlot> CreateBlockedSlots(IReadOnlyList<Specialist> specialists, DateOnly today)
    {
        var slots = new List<BlockedSlot>();

        // Find the next day (within 2 weeks) that Dr. Ozols works, and block his afternoon.
        var ozols = specialists.First(s => s.Id == DrOzolsId);
        for (var offset = 1; offset <= 14; offset++)
        {
            var date = today.AddDays(offset);
            var hours = ozols.GetWorkingHours(date.DayOfWeek);
            if (hours is null)
            {
                continue;
            }

            var midPoint = hours.Value.Start.Add(TimeSpan.FromMinutes(
                (hours.Value.End.ToTimeSpan() - hours.Value.Start.ToTimeSpan()).TotalMinutes / 2));

            slots.Add(new BlockedSlot
            {
                SpecialistId = ozols.Id,
                Start = date.ToDateTime(midPoint),
                End = date.ToDateTime(hours.Value.End),
                ReasonKey = "blocked.onLeave"
            });
            break;
        }

        // And a second one further out for Dr. Kalniņa — a training afternoon.
        var kalnina = specialists.First(s => s.Id == DrKalninaId);
        for (var offset = 6; offset <= 14; offset++)
        {
            var date = today.AddDays(offset);
            var hours = kalnina.GetWorkingHours(date.DayOfWeek);
            if (hours is null)
            {
                continue;
            }

            slots.Add(new BlockedSlot
            {
                SpecialistId = kalnina.Id,
                Start = date.ToDateTime(new TimeOnly(13, 0)),
                End = date.ToDateTime(hours.Value.End),
                ReasonKey = "blocked.training"
            });
            break;
        }

        return slots;
    }
}
