using DbHealthCheckDemo.Core.Models;

namespace DbHealthCheckDemo.Core.Rules;

public sealed class SchemaStalenessRule : IDiagnosticRule
{
    public string RuleId => "schemaStaleness";

    public Finding Evaluate(DatabaseProfile profile)
    {
        var severity = profile.YearsSinceLastSchemaReview switch
        {
            >= 4 => Severity.Critical,
            >= 2 => Severity.Warning,
            _ => Severity.Ok
        };

        return new Finding(
            RuleId,
            Variant: severity == Severity.Ok ? "ok" : "risk",
            severity,
            Args: new object[] { profile.YearsSinceLastSchemaReview });
    }
}
