using DbHealthCheckDemo.Core.Models;
using DbHealthCheckDemo.Core.Rules;

namespace DbHealthCheckDemo.Tests;

public class RulesTests
{
    // Baseline values are all deliberately "healthy" so each test only varies the one
    // field its rule actually reads.
    private static DatabaseProfile Profile(
        long rowCount = 50_000,
        int indexCount = 3,
        double fragmentation = 4,
        bool automatedBackup = true,
        int daysSinceBackup = 1,
        double slowestReportSeconds = 2,
        double yearsSinceSchemaReview = 0.5,
        int concurrentUsers = 2,
        double percentTableScan = 5) => new()
    {
        Id = "test",
        Name = "Test profile",
        LargestTableName = "Widgets",
        LargestTableRowCount = rowCount,
        NonClusteredIndexCountOnLargestTable = indexCount,
        AvgIndexFragmentationPercent = fragmentation,
        HasAutomatedBackupJob = automatedBackup,
        DaysSinceLastFullBackup = daysSinceBackup,
        SlowestReportName = "Nightly Summary",
        SlowestReportSeconds = slowestReportSeconds,
        YearsSinceLastSchemaReview = yearsSinceSchemaReview,
        ConcurrentUserCount = concurrentUsers,
        PercentQueriesUsingTableScan = percentTableScan
    };

    // ---- MissingIndexRule ----

    [Theory]
    [InlineData(50_000, 0, Severity.Ok)]      // small table, no index - not big enough to matter
    [InlineData(150_000, 1, Severity.Warning)]
    [InlineData(600_000, 1, Severity.Critical)]
    [InlineData(600_000, 2, Severity.Ok)]     // large table but adequately indexed
    public void MissingIndexRule_ThresholdsMatchRowCountAndIndexCount(long rowCount, int indexCount, Severity expected)
    {
        var finding = new MissingIndexRule().Evaluate(Profile(rowCount: rowCount, indexCount: indexCount));

        Assert.Equal(expected, finding.Severity);
        Assert.Equal(expected == Severity.Ok ? "ok" : "risk", finding.Variant);
    }

    // ---- FragmentationRule ----

    [Theory]
    [InlineData(9, Severity.Ok)]
    [InlineData(10, Severity.Warning)]
    [InlineData(29, Severity.Warning)]
    [InlineData(30, Severity.Critical)]
    public void FragmentationRule_MatchesMicrosoftRebuildReorganizeThresholds(double fragmentation, Severity expected)
    {
        var finding = new FragmentationRule().Evaluate(Profile(fragmentation: fragmentation));

        Assert.Equal(expected, finding.Severity);
    }

    // ---- BackupRecencyRule ----

    [Fact]
    public void BackupRecencyRule_NoAutomatedJob_IsAlwaysCriticalWithManualVariant()
    {
        var finding = new BackupRecencyRule().Evaluate(Profile(automatedBackup: false));

        Assert.Equal(Severity.Critical, finding.Severity);
        Assert.Equal("manual", finding.Variant);
    }

    [Fact]
    public void BackupRecencyRule_AutomatedAndRecent_IsOk()
    {
        var finding = new BackupRecencyRule().Evaluate(Profile(automatedBackup: true, daysSinceBackup: 2));

        Assert.Equal(Severity.Ok, finding.Severity);
        Assert.Equal("automated-ok", finding.Variant);
    }

    [Fact]
    public void BackupRecencyRule_AutomatedButStale_IsWarning()
    {
        var finding = new BackupRecencyRule().Evaluate(Profile(automatedBackup: true, daysSinceBackup: 8));

        Assert.Equal(Severity.Warning, finding.Severity);
        Assert.Equal("automated-risk", finding.Variant);
    }

    // ---- SlowQueryRule ----

    [Theory]
    [InlineData(9, Severity.Ok)]
    [InlineData(10, Severity.Warning)]
    [InlineData(59, Severity.Warning)]
    [InlineData(60, Severity.Critical)]
    public void SlowQueryRule_MatchesSecondsThresholds(double seconds, Severity expected)
    {
        var finding = new SlowQueryRule().Evaluate(Profile(slowestReportSeconds: seconds));

        Assert.Equal(expected, finding.Severity);
    }

    // ---- SchemaStalenessRule ----

    [Theory]
    [InlineData(1.9, Severity.Ok)]
    [InlineData(2, Severity.Warning)]
    [InlineData(3.9, Severity.Warning)]
    [InlineData(4, Severity.Critical)]
    public void SchemaStalenessRule_MatchesYearsThresholds(double years, Severity expected)
    {
        var finding = new SchemaStalenessRule().Evaluate(Profile(yearsSinceSchemaReview: years));

        Assert.Equal(expected, finding.Severity);
    }

    // ---- ConcurrencyRiskRule ----

    [Theory]
    [InlineData(10, 2, Severity.Ok)]
    [InlineData(25, 6, Severity.Warning)]
    [InlineData(55, 12, Severity.Critical)]
    [InlineData(55, 2, Severity.Ok)]   // high table-scan % but too few concurrent users to matter
    public void ConcurrencyRiskRule_RequiresBothScanPercentAndUserCount(double scanPercent, int users, Severity expected)
    {
        var finding = new ConcurrencyRiskRule().Evaluate(Profile(concurrentUsers: users, percentTableScan: scanPercent));

        Assert.Equal(expected, finding.Severity);
    }
}
