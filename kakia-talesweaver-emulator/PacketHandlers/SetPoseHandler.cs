using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_utils;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.SetPose)]
internal class SetPoseHandler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		using PacketReader reader = new(p.Data);
		reader.Skip(1); // Packet ID
		byte pose = reader.ReadByte(); // 0 = stand | 1 = sit

		using PacketWriter writer = new();
		writer.Write((byte)0x33);
		writer.Write((byte)0x02);
		writer.Write(client.GetCharacter().Id);
		writer.Write(pose);
		writer.Write(new byte[36]);
		client.Broadcast(writer.ToArray(), true);
	}
}
