namespace InventorySyncDemo.Core.Models;

/// <summary>
/// Sales channels that Nordic Fashion House sells through. All channels draw from a
/// single shared stock pool - that shared-pool behavior is the core point of this demo.
/// </summary>
public enum Channel
{
    RetailStoreA,
    RetailStoreB,
    LivestreamOnline
}

public static class ChannelExtensions
{
    public static string DisplayName(this Channel channel) => channel switch
    {
        Channel.RetailStoreA => "Retail Store A",
        Channel.RetailStoreB => "Retail Store B",
        Channel.LivestreamOnline => "Livestream / Online Orders",
        _ => channel.ToString()
    };
}
