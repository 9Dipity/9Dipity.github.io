namespace DbHealthCheckDemo.Core.Models;

/// <summary>
/// A seeded stand-in for the kind of stats a real DBA-style audit gathers before drawing
/// any conclusion. RowCount, HasAutomatedBackupJob, and DaysSinceLastFullBackup are the
/// three fields the demo UI lets a visitor adjust - everything else stays fixed per
/// scenario so the story (retail order DB, clinic scheduling DB, distribution reporting
/// DB) stays coherent while the visitor can still prove the engine isn't hardcoded.
/// </summary>
public sealed class DatabaseProfile
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string LargestTableName { get; init; }
    public required long LargestTableRowCount { get; set; }
    public required int NonClusteredIndexCountOnLargestTable { get; init; }
    public required double AvgIndexFragmentationPercent { get; init; }
    public required bool HasAutomatedBackupJob { get; set; }
    public required int DaysSinceLastFullBackup { get; set; }
    public required string SlowestReportName { get; init; }
    public required double SlowestReportSeconds { get; init; }
    public required double YearsSinceLastSchemaReview { get; init; }
    public required int ConcurrentUserCount { get; init; }
    public required double PercentQueriesUsingTableScan { get; init; }

    public DatabaseProfile Clone() => new()
    {
        Id = Id,
        Name = Name,
        LargestTableName = LargestTableName,
        LargestTableRowCount = LargestTableRowCount,
        NonClusteredIndexCountOnLargestTable = NonClusteredIndexCountOnLargestTable,
        AvgIndexFragmentationPercent = AvgIndexFragmentationPercent,
        HasAutomatedBackupJob = HasAutomatedBackupJob,
        DaysSinceLastFullBackup = DaysSinceLastFullBackup,
        SlowestReportName = SlowestReportName,
        SlowestReportSeconds = SlowestReportSeconds,
        YearsSinceLastSchemaReview = YearsSinceLastSchemaReview,
        ConcurrentUserCount = ConcurrentUserCount,
        PercentQueriesUsingTableScan = PercentQueriesUsingTableScan
    };
}
