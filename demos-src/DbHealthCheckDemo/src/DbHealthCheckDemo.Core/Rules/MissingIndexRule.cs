using DbHealthCheckDemo.Core.Models;

namespace DbHealthCheckDemo.Core.Rules;

public sealed class MissingIndexRule : IDiagnosticRule
{
    public string RuleId => "missingIndex";

    public Finding Evaluate(DatabaseProfile profile)
    {
        var severity = Severity.Ok;
        if (profile.NonClusteredIndexCountOnLargestTable <= 1)
        {
            if (profile.LargestTableRowCount >= 500_000) severity = Severity.Critical;
            else if (profile.LargestTableRowCount >= 100_000) severity = Severity.Warning;
        }

        return new Finding(
            RuleId,
            Variant: severity == Severity.Ok ? "ok" : "risk",
            severity,
            Args: new object[] { profile.LargestTableName, profile.LargestTableRowCount, profile.NonClusteredIndexCountOnLargestTable });
    }
}
