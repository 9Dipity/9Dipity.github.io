namespace ServiceBoardDemo.Core.Models;

/// <summary>
/// One part a job needs. InStock is shop-wide - MarkPartReceived flips it for every
/// job that needs that part name, since a part arriving isn't specific to one ticket.
/// </summary>
public sealed class PartRequirement
{
    public required string PartName { get; init; }
    public required int Quantity { get; init; }
    public bool InStock { get; set; }
}
