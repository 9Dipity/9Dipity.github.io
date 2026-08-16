using ServiceBoardDemo.Core.Models;

namespace ServiceBoardDemo.Core.Services;

public interface IJobBoardService
{
    /// <summary>Active (not yet picked up) jobs, in creation order.</summary>
    IReadOnlyList<RepairJob> Jobs { get; }

    int CompletedTodayCount { get; }

    /// <summary>Id of the job most recently added or moved, for the client to flash its card.</summary>
    Guid? LastChangedJobId { get; }

    event Action? JobsChanged;

    /// <summary>
    /// Moves a job to the next pipeline stage, automatically skipping AwaitingParts when
    /// the job has no parts or all its parts are already in stock. Advancing past Ready
    /// completes the job and removes it from the active board.
    /// </summary>
    void AdvanceStatus(Guid jobId);

    Guid AddJob(string customerName, string vehicleDescription, string issueDescription, IReadOnlyList<PartRequirement>? parts = null);

    /// <summary>Sets a part in-stock shop-wide for every active job that needs it by name.</summary>
    void MarkPartReceived(string partName);

    IReadOnlyList<PartDemand> GetPartsDemand();

    void Reset();
}
