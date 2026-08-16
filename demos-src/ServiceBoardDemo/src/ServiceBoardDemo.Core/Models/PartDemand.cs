namespace ServiceBoardDemo.Core.Models;

/// <summary>
/// Aggregated view for the Parts page: one row per part name currently blocking at
/// least one active job, with the total quantity needed and which jobs are waiting.
/// </summary>
public sealed record PartDemand(string PartName, int TotalQuantity, IReadOnlyList<string> BlockingJobNumbers);
