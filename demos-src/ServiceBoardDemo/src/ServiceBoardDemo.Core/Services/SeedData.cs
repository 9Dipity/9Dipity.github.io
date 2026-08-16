using ServiceBoardDemo.Core.Models;

namespace ServiceBoardDemo.Core.Services;

/// <summary>
/// ~10 fictional jobs spread across the pipeline so every column has something in it
/// on first load, including two AwaitingParts jobs that are genuinely blocked.
/// </summary>
internal static class SeedData
{
    internal static List<RepairJob> BuildJobs()
    {
        var now = DateTimeOffset.UtcNow;

        RepairJob Job(
            string number, string customer, string vehicle, string issue, JobStatus status,
            string? tech, decimal? estimate, List<PartRequirement>? parts = null, int hoursAgo = 0) =>
            new()
            {
                Id = Guid.NewGuid(),
                JobNumber = number,
                CustomerName = customer,
                VehicleDescription = vehicle,
                IssueDescription = issue,
                Status = status,
                TechnicianName = tech,
                EstimatedCost = estimate,
                Parts = parts ?? new List<PartRequirement>(),
                CreatedAt = now.AddHours(-hoursAgo),
                StatusUpdatedAt = now.AddHours(-hoursAgo / 2.0)
            };

        return new List<RepairJob>
        {
            Job("SV-241", "Jānis Kalniņš", "VW Golf Mk7, 2016, GJ-4471",
                "Engine warning light on, rough idle at startup.",
                JobStatus.Intake, null, null, hoursAgo: 1),

            Job("SV-242", "Līga Ozoliņa", "Škoda Octavia, 2019, LV-9012",
                "Customer reports grinding noise from front-left wheel when braking.",
                JobStatus.Intake, null, null, hoursAgo: 1),

            Job("SV-238", "Andris Bērziņš", "Audi A4, 2015, EK-3305",
                "Annual inspection prep — full check requested before TA.",
                JobStatus.Diagnosis, "Māris Vītols", 85m, hoursAgo: 4),

            Job("SV-236", "SIA \"Baltic Logistics\"", "Ford Transit, 2018, GF-7710 (fleet van 3)",
                "AC not cooling, possible compressor fault.",
                JobStatus.Diagnosis, "Māris Vītols", null, hoursAgo: 5),

            Job("SV-233", "Ilze Krūmiņa", "BMW 320d, 2014, KK-2298",
                "Timing belt replacement — belt and tensioner confirmed worn on inspection.",
                JobStatus.AwaitingParts, "Edgars Liepa", 420m,
                parts: new List<PartRequirement>
                {
                    new() { PartName = "Timing belt kit (BMW N47)", Quantity = 1, InStock = false },
                    new() { PartName = "Water pump", Quantity = 1, InStock = false }
                },
                hoursAgo: 26),

            Job("SV-234", "Uģis Ozols", "Toyota Auris, 2013, TE-5541",
                "Front brake pads and discs — pads under 2mm, discs scored.",
                JobStatus.AwaitingParts, "Edgars Liepa", 190m,
                parts: new List<PartRequirement>
                {
                    new() { PartName = "Front brake discs (pair)", Quantity = 1, InStock = false },
                    new() { PartName = "Front brake pads", Quantity = 1, InStock = true }
                },
                hoursAgo: 22),

            Job("SV-229", "Sanita Feldmane", "Mazda CX-5, 2020, RE-1187",
                "Battery replacement — cold-start failure confirmed, battery tested dead.",
                JobStatus.InProgress, "Kārlis Ozola", 145m,
                parts: new List<PartRequirement>
                {
                    new() { PartName = "12V AGM battery", Quantity = 1, InStock = true }
                },
                hoursAgo: 30),

            Job("SV-230", "Roberts Vanags", "Opel Astra, 2017, DA-8823",
                "Oil and filter service, cabin filter replacement.",
                JobStatus.InProgress, "Kārlis Ozola", 65m, hoursAgo: 28),

            Job("SV-224", "Marta Siliņa", "Renault Clio, 2012, VA-4409",
                "Clutch replacement — slipping under load, confirmed at diagnosis.",
                JobStatus.Ready, "Edgars Liepa", 380m, hoursAgo: 48),

            Job("SV-221", "Dainis Ābele", "Volvo XC60, 2016, GT-6650",
                "Full brake fluid flush and rear pad replacement.",
                JobStatus.Ready, "Māris Vītols", 210m, hoursAgo: 50),
        };
    }
}
