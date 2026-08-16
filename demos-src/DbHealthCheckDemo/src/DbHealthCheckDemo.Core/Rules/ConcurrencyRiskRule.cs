using DbHealthCheckDemo.Core.Models;

namespace DbHealthCheckDemo.Core.Rules;

public sealed class ConcurrencyRiskRule : IDiagnosticRule
{
    public string RuleId => "concurrencyRisk";

    public Finding Evaluate(DatabaseProfile profile)
    {
        var severity = Severity.Ok;
        if (profile.PercentQueriesUsingTableScan >= 40 && profile.ConcurrentUserCount >= 10)
        {
            severity = Severity.Critical;
        }
        else if (profile.PercentQueriesUsingTableScan >= 20 && profile.ConcurrentUserCount >= 5)
        {
            severity = Severity.Warning;
        }

        return new Finding(
            RuleId,
            Variant: severity == Severity.Ok ? "ok" : "risk",
            severity,
            Args: new object[] { profile.PercentQueriesUsingTableScan, profile.ConcurrentUserCount });
    }
}
