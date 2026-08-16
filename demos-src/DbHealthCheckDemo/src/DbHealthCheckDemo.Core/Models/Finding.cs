namespace DbHealthCheckDemo.Core.Models;

/// <summary>
/// One rule's verdict against a profile. Carries no localized text - Variant selects
/// which template the Client's localizer renders (e.g. "ok" vs "risk", or
/// "manual"/"automated-ok"/"automated-risk" for backup coverage), and Args are the raw
/// numbers/strings plugged into that template via string.Format.
/// </summary>
public sealed record Finding(
    string RuleId,
    string Variant,
    Severity Severity,
    IReadOnlyList<object> Args);
