namespace DbHealthCheckDemo.Core.Models;

public sealed record AuditResult(
    string ProfileName,
    IReadOnlyList<Finding> Findings,
    Severity OverallSeverity,
    AuditTier RecommendedTier);
