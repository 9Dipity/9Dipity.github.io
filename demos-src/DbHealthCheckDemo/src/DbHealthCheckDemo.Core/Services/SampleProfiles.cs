using DbHealthCheckDemo.Core.Models;

namespace DbHealthCheckDemo.Core.Services;

/// <summary>
/// Three seeded scenarios tied to the same three verticals as the site's other
/// illustrative case studies. Numbers are picked so the three land at genuinely
/// different tiers (Quick Check / Standard Audit / Deep Dive) - the point isn't to
/// always show the scariest possible result, it's to show the engine actually
/// discriminates between a mostly-healthy system and a badly overdue one.
/// </summary>
public static class SampleProfiles
{
    public static IReadOnlyList<DatabaseProfile> All { get; } = new List<DatabaseProfile>
    {
        new()
        {
            Id = "retail",
            Name = "Retail order-processing database",
            LargestTableName = "Orders",
            LargestTableRowCount = 100_000,
            NonClusteredIndexCountOnLargestTable = 1,
            AvgIndexFragmentationPercent = 12,
            HasAutomatedBackupJob = true,
            DaysSinceLastFullBackup = 1,
            SlowestReportName = "Daily Sales Summary",
            SlowestReportSeconds = 8,
            YearsSinceLastSchemaReview = 1.5,
            ConcurrentUserCount = 4,
            PercentQueriesUsingTableScan = 15
        },
        new()
        {
            Id = "clinic",
            Name = "Clinic scheduling and billing database",
            LargestTableName = "Appointments",
            LargestTableRowCount = 500_000,
            NonClusteredIndexCountOnLargestTable = 1,
            AvgIndexFragmentationPercent = 22,
            HasAutomatedBackupJob = true,
            DaysSinceLastFullBackup = 12,
            SlowestReportName = "Monthly Billing Reconciliation",
            SlowestReportSeconds = 34,
            YearsSinceLastSchemaReview = 3.2,
            ConcurrentUserCount = 7,
            PercentQueriesUsingTableScan = 25
        },
        new()
        {
            Id = "distribution",
            Name = "Distribution reporting database",
            LargestTableName = "PriceHistory",
            LargestTableRowCount = 10_000_000,
            NonClusteredIndexCountOnLargestTable = 0,
            AvgIndexFragmentationPercent = 41,
            HasAutomatedBackupJob = false,
            DaysSinceLastFullBackup = 0,
            SlowestReportName = "Nightly Supplier Price Export",
            SlowestReportSeconds = 95,
            YearsSinceLastSchemaReview = 4.5,
            ConcurrentUserCount = 12,
            PercentQueriesUsingTableScan = 55
        }
    }.AsReadOnly();
}
