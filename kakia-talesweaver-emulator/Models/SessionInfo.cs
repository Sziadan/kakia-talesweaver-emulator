using kakia_talesweaver_packets.Models;

namespace kakia_talesweaver_emulator.Models;

public class SessionInfo
{
	public string AccountId { get; set; } = string.Empty;
	public Character? Character { get; set; }
}
