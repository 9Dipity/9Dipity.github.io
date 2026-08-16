namespace ServiceBoardDemo.Core.Models;

/// <summary>
/// Linear pipeline order. AwaitingParts is skipped automatically by JobBoardService
/// when a job has no blocking parts - see JobBoardService.AdvanceStatus.
/// </summary>
public enum JobStatus
{
    Intake,
    Diagnosis,
    AwaitingParts,
    InProgress,
    Ready
}
