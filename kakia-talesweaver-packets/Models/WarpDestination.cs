namespace kakia_talesweaver_packets.Models;

public class WarpDestination
{
	public ushort MapId { get; set; }
	public ushort ZoneId { get; set; }
	public ObjectPos Position { get; set; } = new();
}
