namespace ServiceBoardDemo.Core.Models;

public sealed class RepairJob
{
    public required Guid Id { get; init; }
    public required string JobNumber { get; init; }
    public required string CustomerName { get; init; }
    public required string VehicleDescription { get; init; }
    public required string IssueDescription { get; init; }
    public JobStatus Status { get; set; } = JobStatus.Intake;
    public string? TechnicianName { get; set; }
    public decimal? EstimatedCost { get; set; }
    public List<PartRequirement> Parts { get; init; } = new();
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset StatusUpdatedAt { get; set; }

    public bool IsBlockedOnParts => Status == JobStatus.AwaitingParts && Parts.Any(p => !p.InStock);
}
