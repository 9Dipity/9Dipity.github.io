using ServiceBoardDemo.Core.Models;
using ServiceBoardDemo.Core.Services;

namespace ServiceBoardDemo.Tests;

public class JobBoardServiceTests
{
    [Fact]
    public void AddJob_PlacesNewJobAtIntake_AndFiresJobsChanged()
    {
        var service = new JobBoardService();
        var fired = false;
        service.JobsChanged += () => fired = true;

        var id = service.AddJob("Test Customer", "Test Vehicle", "Test issue");

        var job = service.Jobs.Single(j => j.Id == id);
        Assert.Equal(JobStatus.Intake, job.Status);
        Assert.True(fired);
        Assert.Equal(id, service.LastChangedJobId);
    }

    [Fact]
    public void AddJob_GeneratesUniqueSequentialJobNumbers()
    {
        var service = new JobBoardService();

        var id1 = service.AddJob("A", "V1", "Issue 1");
        var id2 = service.AddJob("B", "V2", "Issue 2");

        var number1 = service.Jobs.Single(j => j.Id == id1).JobNumber;
        var number2 = service.Jobs.Single(j => j.Id == id2).JobNumber;

        Assert.NotEqual(number1, number2);
    }

    [Fact]
    public void AdvanceStatus_WithNoParts_SkipsAwaitingPartsColumn()
    {
        var service = new JobBoardService();
        var id = service.AddJob("Customer", "Vehicle", "Issue");

        service.AdvanceStatus(id); // Intake -> Diagnosis
        Assert.Equal(JobStatus.Diagnosis, service.Jobs.Single(j => j.Id == id).Status);

        service.AdvanceStatus(id); // Diagnosis -> (AwaitingParts skipped) -> InProgress
        Assert.Equal(JobStatus.InProgress, service.Jobs.Single(j => j.Id == id).Status);
    }

    [Fact]
    public void AdvanceStatus_WithBlockingParts_StopsAtAwaitingParts()
    {
        var service = new JobBoardService();
        var parts = new List<PartRequirement>
        {
            new() { PartName = "Brake pads", Quantity = 1, InStock = false }
        };
        var id = service.AddJob("Customer", "Vehicle", "Issue", parts);

        service.AdvanceStatus(id); // Intake -> Diagnosis
        service.AdvanceStatus(id); // Diagnosis -> AwaitingParts (not skipped, part missing)

        var job = service.Jobs.Single(j => j.Id == id);
        Assert.Equal(JobStatus.AwaitingParts, job.Status);
        Assert.True(job.IsBlockedOnParts);
    }

    [Fact]
    public void AdvanceStatus_PastReady_CompletesJobAndIncrementsCounter()
    {
        var service = new JobBoardService();
        var id = service.AddJob("Customer", "Vehicle", "Issue");

        service.AdvanceStatus(id); // Intake -> Diagnosis
        service.AdvanceStatus(id); // Diagnosis -> InProgress (parts skipped)
        service.AdvanceStatus(id); // InProgress -> Ready
        Assert.Equal(JobStatus.Ready, service.Jobs.Single(j => j.Id == id).Status);

        var completedBefore = service.CompletedTodayCount;
        service.AdvanceStatus(id); // Ready -> completed, removed from board

        Assert.DoesNotContain(service.Jobs, j => j.Id == id);
        Assert.Equal(completedBefore + 1, service.CompletedTodayCount);
    }

    [Fact]
    public void MarkPartReceived_UnblocksJob_ButDoesNotAdvanceStatusAutomatically()
    {
        var service = new JobBoardService();
        var parts = new List<PartRequirement>
        {
            new() { PartName = "Timing belt", Quantity = 1, InStock = false }
        };
        var id = service.AddJob("Customer", "Vehicle", "Issue", parts);
        service.AdvanceStatus(id); // Intake -> Diagnosis
        service.AdvanceStatus(id); // Diagnosis -> AwaitingParts

        Assert.True(service.Jobs.Single(j => j.Id == id).IsBlockedOnParts);

        service.MarkPartReceived("Timing belt");

        var job = service.Jobs.Single(j => j.Id == id);
        Assert.False(job.IsBlockedOnParts);
        Assert.Equal(JobStatus.AwaitingParts, job.Status); // still requires an explicit Advance click
    }

    [Fact]
    public void MarkPartReceived_OnlyAffectsMatchingPartName_AcrossJobs()
    {
        var service = new JobBoardService();
        var jobAParts = new List<PartRequirement> { new() { PartName = "Alternator", Quantity = 1, InStock = false } };
        var jobBParts = new List<PartRequirement> { new() { PartName = "Radiator", Quantity = 1, InStock = false } };
        var idA = service.AddJob("A", "V1", "Issue A", jobAParts);
        var idB = service.AddJob("B", "V2", "Issue B", jobBParts);
        service.AdvanceStatus(idA);
        service.AdvanceStatus(idA);
        service.AdvanceStatus(idB);
        service.AdvanceStatus(idB);

        service.MarkPartReceived("Alternator");

        Assert.False(service.Jobs.Single(j => j.Id == idA).IsBlockedOnParts);
        Assert.True(service.Jobs.Single(j => j.Id == idB).IsBlockedOnParts);
    }

    [Fact]
    public void GetPartsDemand_AggregatesQuantityAndJobNumbers_ForUnstockedPartsOnly()
    {
        var service = new JobBoardService();
        var jobAParts = new List<PartRequirement>
        {
            new() { PartName = "Front brake discs", Quantity = 2, InStock = false }
        };
        var jobBParts = new List<PartRequirement>
        {
            new() { PartName = "Front brake discs", Quantity = 1, InStock = false },
            new() { PartName = "Oil filter", Quantity = 1, InStock = true } // already in stock, shouldn't appear
        };
        service.AddJob("A", "V1", "Issue A", jobAParts);
        service.AddJob("B", "V2", "Issue B", jobBParts);

        var demand = service.GetPartsDemand();

        var discs = Assert.Single(demand, d => d.PartName == "Front brake discs");
        Assert.Equal(3, discs.TotalQuantity);
        Assert.Equal(2, discs.BlockingJobNumbers.Count);
        Assert.DoesNotContain(demand, d => d.PartName == "Oil filter");
    }

    [Fact]
    public void GetPartsDemand_ExcludesCompletedJobs()
    {
        var service = new JobBoardService();
        var parts = new List<PartRequirement> { new() { PartName = "Clutch kit", Quantity = 1, InStock = false } };
        var id = service.AddJob("A", "V1", "Issue", parts);
        service.AdvanceStatus(id); // Diagnosis
        service.AdvanceStatus(id); // AwaitingParts
        service.MarkPartReceived("Clutch kit");
        service.AdvanceStatus(id); // InProgress
        service.AdvanceStatus(id); // Ready
        service.AdvanceStatus(id); // Completed, removed

        Assert.DoesNotContain(service.GetPartsDemand(), d => d.PartName == "Clutch kit");
    }

    [Fact]
    public void Reset_RestoresSeedJobsAndClearsCompletedCounter()
    {
        var service = new JobBoardService();
        var seededCount = service.Jobs.Count;
        var id = service.AddJob("Extra", "Vehicle", "Issue");
        service.AdvanceStatus(id);
        service.AdvanceStatus(id);
        service.AdvanceStatus(id);
        service.AdvanceStatus(id); // completes one job

        Assert.True(service.CompletedTodayCount > 0);

        service.Reset();

        Assert.Equal(seededCount, service.Jobs.Count);
        Assert.Equal(0, service.CompletedTodayCount);
        Assert.Null(service.LastChangedJobId);
    }

    [Fact]
    public void SeededData_HasJobsInEveryActiveStatus()
    {
        var service = new JobBoardService();
        var statuses = service.Jobs.Select(j => j.Status).Distinct().ToList();

        foreach (var status in new[] { JobStatus.Intake, JobStatus.Diagnosis, JobStatus.AwaitingParts, JobStatus.InProgress, JobStatus.Ready })
        {
            Assert.Contains(status, statuses);
        }
    }

    [Fact]
    public void SeededAwaitingPartsJobs_AreGenuinelyBlocked()
    {
        var service = new JobBoardService();
        var blockedJobs = service.Jobs.Where(j => j.Status == JobStatus.AwaitingParts).ToList();

        Assert.NotEmpty(blockedJobs);
        Assert.All(blockedJobs, j => Assert.True(j.IsBlockedOnParts));
    }
}
