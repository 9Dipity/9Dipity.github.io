using ServiceBoardDemo.Core.Models;

namespace ServiceBoardDemo.Core.Services;

/// <summary>
/// In-memory job board. Register as a singleton so the board page and the parts page
/// read/write through the exact same state and both re-render via JobsChanged.
/// </summary>
public sealed class JobBoardService : IJobBoardService
{
    private static readonly JobStatus[] Pipeline =
    {
        JobStatus.Intake, JobStatus.Diagnosis, JobStatus.AwaitingParts, JobStatus.InProgress, JobStatus.Ready
    };

    private readonly object _lock = new();
    private List<RepairJob> _jobs = new();
    private int _nextJobNumber;

    public event Action? JobsChanged;

    public Guid? LastChangedJobId { get; private set; }

    public int CompletedTodayCount { get; private set; }

    public IReadOnlyList<RepairJob> Jobs
    {
        get
        {
            lock (_lock)
            {
                return _jobs.ToList();
            }
        }
    }

    public JobBoardService()
    {
        SeedJobs();
    }

    private void SeedJobs()
    {
        lock (_lock)
        {
            _jobs = SeedData.BuildJobs();
            _nextJobNumber = _jobs
                .Select(j => int.TryParse(j.JobNumber.Split('-').Last(), out var n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max() + 1;
        }
        CompletedTodayCount = 0;
    }

    public void AdvanceStatus(Guid jobId)
    {
        lock (_lock)
        {
            var job = _jobs.FirstOrDefault(j => j.Id == jobId);
            if (job is null) return;

            var index = Array.IndexOf(Pipeline, job.Status);
            var nextIndex = index + 1;

            while (nextIndex < Pipeline.Length &&
                   Pipeline[nextIndex] == JobStatus.AwaitingParts &&
                   !job.Parts.Any(p => !p.InStock))
            {
                nextIndex++;
            }

            if (nextIndex >= Pipeline.Length)
            {
                _jobs.Remove(job);
                CompletedTodayCount++;
            }
            else
            {
                job.Status = Pipeline[nextIndex];
                job.StatusUpdatedAt = DateTimeOffset.UtcNow;
            }

            LastChangedJobId = jobId;
        }

        JobsChanged?.Invoke();
    }

    public void RevertStatus(Guid jobId)
    {
        lock (_lock)
        {
            var job = _jobs.FirstOrDefault(j => j.Id == jobId);
            if (job is null) return;

            var index = Array.IndexOf(Pipeline, job.Status);
            var prevIndex = index - 1;

            while (prevIndex >= 0 &&
                   Pipeline[prevIndex] == JobStatus.AwaitingParts &&
                   !job.Parts.Any(p => !p.InStock))
            {
                prevIndex--;
            }

            if (prevIndex < 0) return; // already at Intake, nothing earlier to revert to

            job.Status = Pipeline[prevIndex];
            job.StatusUpdatedAt = DateTimeOffset.UtcNow;
            LastChangedJobId = jobId;
        }

        JobsChanged?.Invoke();
    }

    public Guid AddJob(string customerName, string vehicleDescription, string issueDescription, IReadOnlyList<PartRequirement>? parts = null)
    {
        var job = new RepairJob
        {
            Id = Guid.NewGuid(),
            JobNumber = $"SV-{_nextJobNumber}",
            CustomerName = customerName,
            VehicleDescription = vehicleDescription,
            IssueDescription = issueDescription,
            Status = JobStatus.Intake,
            Parts = parts?.ToList() ?? new List<PartRequirement>(),
            CreatedAt = DateTimeOffset.UtcNow,
            StatusUpdatedAt = DateTimeOffset.UtcNow
        };

        lock (_lock)
        {
            _jobs.Add(job);
            _nextJobNumber++;
        }

        LastChangedJobId = job.Id;
        JobsChanged?.Invoke();
        return job.Id;
    }

    public void MarkPartReceived(string partName)
    {
        lock (_lock)
        {
            foreach (var job in _jobs)
            {
                foreach (var part in job.Parts.Where(p => p.PartName == partName))
                {
                    part.InStock = true;
                }
            }
        }

        JobsChanged?.Invoke();
    }

    public IReadOnlyList<PartDemand> GetPartsDemand()
    {
        lock (_lock)
        {
            return _jobs
                .SelectMany(j => j.Parts.Where(p => !p.InStock).Select(p => (Job: j, Part: p)))
                .GroupBy(x => x.Part.PartName)
                .Select(g => new PartDemand(
                    g.Key,
                    g.Sum(x => x.Part.Quantity),
                    g.Select(x => x.Job.JobNumber).Distinct().OrderBy(n => n).ToList()))
                .OrderBy(d => d.PartName)
                .ToList();
        }
    }

    public void Reset()
    {
        SeedJobs();
        LastChangedJobId = null;
        JobsChanged?.Invoke();
    }
}
