using DbHealthCheckDemo.Core.Models;
using DbHealthCheckDemo.Core.Rules;

namespace DbHealthCheckDemo.Core.Services;

/// <summary>
/// Runs every rule against a profile and maps the result to a recommended audit tier.
/// The tier mapping is deliberately simple and stated here in one place so it's easy to
/// defend on a call: 2+ critical findings needs a Deep Dive; any critical finding, or
/// three or more warnings, needs a Standard Audit; anything lighter (including a clean
/// bill of health) is what a Quick Check is for.
/// </summary>
public sealed class DiagnosticEngine : IDiagnosticEngine
{
    private static readonly IReadOnlyList<IDiagnosticRule> Rules = new IDiagnosticRule[]
    {
        new MissingIndexRule(),
        new FragmentationRule(),
        new BackupRecencyRule(),
        new SlowQueryRule(),
        new SchemaStalenessRule(),
        new ConcurrencyRiskRule()
    };

    public AuditResult Run(DatabaseProfile profile)
    {
        var findings = Rules.Select(rule => rule.Evaluate(profile)).ToList();

        var overallSeverity = findings.Count == 0 ? Severity.Ok : findings.Max(f => f.Severity);
        var criticalCount = findings.Count(f => f.Severity == Severity.Critical);
        var warningCount = findings.Count(f => f.Severity == Severity.Warning);

        var tier = criticalCount >= 2
            ? AuditTier.DeepDive
            : criticalCount >= 1 || warningCount >= 3
                ? AuditTier.StandardAudit
                : AuditTier.QuickCheck;

        return new AuditResult(profile.Name, findings, overallSeverity, tier);
    }
}
