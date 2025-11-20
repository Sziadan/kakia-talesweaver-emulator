using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_utils;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.CheckName)]
public class CheckNameHandler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		using PacketReader reader = new(p.Data);
		reader.Skip(1); // Skip Packet ID
		string nameToCheck = reader.ReadPrefixedString();

		// Validate that name is available

		using PacketWriter writer = new();
		writer.Write((byte)0x02);
		writer.Write((byte)0x58);
		writer.Write((byte)0x00);

		string nameAvailable = "NEW_CHARACTER_ID_AVAILABLE";
		writer.Write(nameAvailable.Length);
		writer.WriteRawASCII(nameAvailable);

		client.Send(writer.ToArray(), CancellationToken.None).Wait();
	}
}
