using DbHealthCheckDemo.Core.Models;

namespace DbHealthCheckDemo.Core.Rules;

/// <summary>Thresholds match Microsoft's own documented rebuild (&gt;=30%) / reorganize (&gt;=10%) guidance.</summary>
public sealed class FragmentationRule : IDiagnosticRule
{
    public string RuleId => "fragmentation";

    public Finding Evaluate(DatabaseProfile profile)
    {
        var severity = profile.AvgIndexFragmentationPercent switch
        {
            >= 30 => Severity.Critical,
            >= 10 => Severity.Warning,
            _ => Severity.Ok
        };

        return new Finding(
            RuleId,
            Variant: severity == Severity.Ok ? "ok" : "risk",
            severity,
            Args: new object[] { profile.AvgIndexFragmentationPercent });
    }
}
