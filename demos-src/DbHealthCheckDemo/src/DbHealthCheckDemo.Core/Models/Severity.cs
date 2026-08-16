namespace DbHealthCheckDemo.Core.Models;

/// <summary>Ordered so callers can take the max across findings with plain comparison.</summary>
public enum Severity
{
    Ok = 0,
    Info = 1,
    Warning = 2,
    Critical = 3
}
