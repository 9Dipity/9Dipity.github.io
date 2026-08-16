using DbHealthCheckDemo.Core.Models;

namespace DbHealthCheckDemo.Core.Rules;

public sealed class BackupRecencyRule : IDiagnosticRule
{
    public string RuleId => "backupRecency";

    public Finding Evaluate(DatabaseProfile profile)
    {
        if (!profile.HasAutomatedBackupJob)
        {
            return new Finding(RuleId, "manual", Severity.Critical, Args: Array.Empty<object>());
        }

        if (profile.DaysSinceLastFullBackup > 7)
        {
            return new Finding(RuleId, "automated-risk", Severity.Warning, Args: new object[] { profile.DaysSinceLastFullBackup });
        }

        return new Finding(RuleId, "automated-ok", Severity.Ok, Args: new object[] { profile.DaysSinceLastFullBackup });
    }
}
