using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_packets.Packets;
using kakia_talesweaver_utils;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.UpdateDirection)]
public class DirectionHandler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		if (p.Data.Length != 2)
			return;

		var character = client.GetCharacter();
		character.Direction = p.Data[1];

		// temporary fix for spawn position desync after movement
		character.SpawnCharacterPacket?.Position.Direction = p.Data[1];

		using PacketWriter pw = new();
		pw.Write((byte)0x11);
		pw.Write(character.Id);
		pw.Write(character.Direction);

		client.Broadcast(pw.ToArray());
	}
}
