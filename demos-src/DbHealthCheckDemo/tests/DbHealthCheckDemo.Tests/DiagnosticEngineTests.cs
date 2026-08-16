using DbHealthCheckDemo.Core.Models;
using DbHealthCheckDemo.Core.Services;

namespace DbHealthCheckDemo.Tests;

public class DiagnosticEngineTests
{
    private static DatabaseProfile CleanProfile() => new()
    {
        Id = "clean",
        Name = "Clean profile",
        LargestTableName = "Widgets",
        LargestTableRowCount = 50_000,
        NonClusteredIndexCountOnLargestTable = 3,
        AvgIndexFragmentationPercent = 4,
        HasAutomatedBackupJob = true,
        DaysSinceLastFullBackup = 1,
        SlowestReportName = "Nightly Summary",
        SlowestReportSeconds = 2,
        YearsSinceLastSchemaReview = 0.5,
        ConcurrentUserCount = 2,
        PercentQueriesUsingTableScan = 5
    };

    [Fact]
    public void Run_AllHealthyProfile_ReturnsSixFindingsAllOk_AndRecommendsQuickCheck()
    {
        var result = new DiagnosticEngine().Run(CleanProfile());

        Assert.Equal(6, result.Findings.Count);
        Assert.All(result.Findings, f => Assert.Equal(Severity.Ok, f.Severity));
        Assert.Equal(Severity.Ok, result.OverallSeverity);
        Assert.Equal(AuditTier.QuickCheck, result.RecommendedTier);
    }

    [Fact]
    public void Run_TwoOrMoreCriticalFindings_RecommendsDeepDive()
    {
        // Force two genuinely critical findings: no indexing AND no backups.
        var forcedProfile = new DatabaseProfile
        {
            Id = "critical-x2",
            Name = "Two critical",
            LargestTableName = "Widgets",
            LargestTableRowCount = 800_000,
            NonClusteredIndexCountOnLargestTable = 0,
            AvgIndexFragmentationPercent = 4,
            HasAutomatedBackupJob = false,
            DaysSinceLastFullBackup = 0,
            SlowestReportName = "Nightly Summary",
            SlowestReportSeconds = 2,
            YearsSinceLastSchemaReview = 0.5,
            ConcurrentUserCount = 2,
            PercentQueriesUsingTableScan = 5
        };

        var result = new DiagnosticEngine().Run(forcedProfile);

        Assert.True(result.Findings.Count(f => f.Severity == Severity.Critical) >= 2);
        Assert.Equal(AuditTier.DeepDive, result.RecommendedTier);
    }

    [Fact]
    public void Run_OneCriticalFinding_RecommendsStandardAudit()
    {
        var profile = CleanProfile();
        profile.HasAutomatedBackupJob = false; // exactly one critical finding, everything else healthy

        var result = new DiagnosticEngine().Run(profile);

        Assert.Equal(1, result.Findings.Count(f => f.Severity == Severity.Critical));
        Assert.Equal(AuditTier.StandardAudit, result.RecommendedTier);
    }

    [Fact]
    public void Run_ThreeOrMoreWarningsWithNoCriticals_RecommendsStandardAudit()
    {
        var profile = new DatabaseProfile
        {
            Id = "warnings",
            Name = "Three warnings",
            LargestTableName = "Orders",
            LargestTableRowCount = 150_000,       // missingIndex -> warning
            NonClusteredIndexCountOnLargestTable = 1,
            AvgIndexFragmentationPercent = 15,    // fragmentation -> warning
            HasAutomatedBackupJob = true,
            DaysSinceLastFullBackup = 10,         // backupRecency -> warning
            SlowestReportName = "Report",
            SlowestReportSeconds = 2,
            YearsSinceLastSchemaReview = 0.5,
            ConcurrentUserCount = 2,
            PercentQueriesUsingTableScan = 5
        };

        var result = new DiagnosticEngine().Run(profile);

        Assert.Equal(0, result.Findings.Count(f => f.Severity == Severity.Critical));
        Assert.True(result.Findings.Count(f => f.Severity == Severity.Warning) >= 3);
        Assert.Equal(AuditTier.StandardAudit, result.RecommendedTier);
    }

    [Fact]
    public void AdjustingRowCountAndRerunning_GenuinelyChangesTheFindings()
    {
        // This is the demo's core honesty claim: the visitor-adjustable inputs aren't
        // decorative, the engine actually recomputes from them.
        var profile = new DatabaseProfile
        {
            Id = "adjustable",
            Name = "Adjustable profile",
            LargestTableName = "Widgets",
            LargestTableRowCount = 50_000,
            NonClusteredIndexCountOnLargestTable = 1,
            AvgIndexFragmentationPercent = 4,
            HasAutomatedBackupJob = true,
            DaysSinceLastFullBackup = 1,
            SlowestReportName = "Nightly Summary",
            SlowestReportSeconds = 2,
            YearsSinceLastSchemaReview = 0.5,
            ConcurrentUserCount = 2,
            PercentQueriesUsingTableScan = 5
        };

        var before = new DiagnosticEngine().Run(profile);
        var missingIndexBefore = before.Findings.Single(f => f.RuleId == "missingIndex");
        Assert.Equal(Severity.Ok, missingIndexBefore.Severity);

        profile.LargestTableRowCount = 700_000;
        var after = new DiagnosticEngine().Run(profile);
        var missingIndexAfter = after.Findings.Single(f => f.RuleId == "missingIndex");

        Assert.Equal(Severity.Critical, missingIndexAfter.Severity);
        Assert.NotEqual(before.RecommendedTier, after.RecommendedTier);
    }

    [Fact]
    public void TogglingBackupAutomation_ChangesVariantAndSeverityImmediately()
    {
        var profile = CleanProfile();
        var automated = new DiagnosticEngine().Run(profile).Findings.Single(f => f.RuleId == "backupRecency");
        Assert.Equal(Severity.Ok, automated.Severity);

        profile.HasAutomatedBackupJob = false;
        var manual = new DiagnosticEngine().Run(profile).Findings.Single(f => f.RuleId == "backupRecency");

        Assert.Equal(Severity.Critical, manual.Severity);
        Assert.Equal("manual", manual.Variant);
    }

    [Fact]
    public void SampleProfiles_HasExactlyThreeScenarios_TiedToTheThreeSiteVerticals()
    {
        var ids = SampleProfiles.All.Select(p => p.Id).ToList();

        Assert.Equal(3, ids.Count);
        Assert.Contains("retail", ids);
        Assert.Contains("clinic", ids);
        Assert.Contains("distribution", ids);
    }

    [Theory]
    [InlineData("retail", AuditTier.QuickCheck)]
    [InlineData("clinic", AuditTier.StandardAudit)]
    [InlineData("distribution", AuditTier.DeepDive)]
    public void SeededScenarios_LandAtTheirIntendedTier(string scenarioId, AuditTier expectedTier)
    {
        var profile = SampleProfiles.All.Single(p => p.Id == scenarioId);

        var result = new DiagnosticEngine().Run(profile);

        Assert.Equal(expectedTier, result.RecommendedTier);
    }

    [Fact]
    public void SeededScenarios_AreNotAllTheSameSeverity()
    {
        // Guards against a lazy "everything is critical" demo that doesn't actually
        // discriminate between a healthy and an unhealthy system.
        var severities = SampleProfiles.All
            .Select(p => new DiagnosticEngine().Run(p).OverallSeverity)
            .Distinct()
            .ToList();

        Assert.True(severities.Count > 1);
    }
}
