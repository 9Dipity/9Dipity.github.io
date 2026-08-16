using DbHealthCheckDemo.Core.Models;

namespace DbHealthCheckDemo.Core.Rules;

/// <summary>
/// One dimension of the audit. Implementations hold no localized text - only thresholds
/// and the profile fields needed to evaluate them. Finding.Args carries the raw numbers
/// so the Client layer builds the sentence via Localizer.T + string.Format, the same
/// separation the other two demos use for parameterized strings.
/// </summary>
public interface IDiagnosticRule
{
    string RuleId { get; }
    Finding Evaluate(DatabaseProfile profile);
}
