using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_packets.Packets;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.Chat)]
public class ChatHandler : PacketHandler
{
	
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var packet = ChatPacket.FromBytes(p.Data);
		packet.CharacterId = client.GetCharacter().Id;
		client.Broadcast(packet.ToBytes(), includeSelf: true);
	}
}