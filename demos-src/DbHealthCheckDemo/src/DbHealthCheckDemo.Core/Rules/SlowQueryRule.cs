using DbHealthCheckDemo.Core.Models;

namespace DbHealthCheckDemo.Core.Rules;

public sealed class SlowQueryRule : IDiagnosticRule
{
    public string RuleId => "slowQuery";

    public Finding Evaluate(DatabaseProfile profile)
    {
        var severity = profile.SlowestReportSeconds switch
        {
            >= 60 => Severity.Critical,
            >= 10 => Severity.Warning,
            _ => Severity.Ok
        };

        return new Finding(
            RuleId,
            Variant: severity == Severity.Ok ? "ok" : "risk",
            severity,
            Args: new object[] { profile.SlowestReportName, profile.SlowestReportSeconds });
    }
}
