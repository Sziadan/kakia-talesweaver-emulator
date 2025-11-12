using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_packets.Packets;
using kakia_talesweaver_utils;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.SelectCharacter)]
public class SelectCharacterHandler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		using PacketReader pr = new(p.Data);
		
		// Use later to match character
		var characterName = pr.ReadPrefixedString();

		CharLoginSecurityPacket clsp = new() { charLoginSecurityType = CharLoginSecurityType.RequestSecurityCode };
		client.Send(clsp.ToBytes(), CancellationToken.None).Wait();
	}
}
