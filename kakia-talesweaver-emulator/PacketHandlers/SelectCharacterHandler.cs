using kakia_talesweaver_emulator.DB;
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
		pr.Skip(1); // Skip Packet ID


		var characterName = pr.ReadPrefixedString();
		client.SetCharacter(characterName);

		CharLoginSecurityPacket clsp = new() { charLoginSecurityType = CharLoginSecurityType.RequestSecurityCode };
		client.Send(clsp.ToBytes(), CancellationToken.None).Wait();
	}
}
