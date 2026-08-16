using DbHealthCheckDemo.Core.Models;

namespace DbHealthCheckDemo.Core.Services;

public interface IDiagnosticEngine
{
    AuditResult Run(DatabaseProfile profile);
}
