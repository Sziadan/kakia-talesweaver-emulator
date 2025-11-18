using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_utils;
using kakia_talesweaver_utils.Extensions;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.Attack)]
public class AttackEnemyHandler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		client.Send("4A 00 00 00 00 00 00".ToByteArray(), CancellationToken.None).Wait();
		client.Send("17 00".ToByteArray(), CancellationToken.None).Wait();

		using PacketWriter pw = new();
		pw.Write((byte)0x48);
		pw.Write((byte)0);
		pw.Write(client.GetCharacter().Id);
		pw.Write((byte)0xFF);

		client.Send(pw.ToArray(), CancellationToken.None).Wait();
		client.Send("6C 03 E7".ToByteArray(), CancellationToken.None).Wait();
	}
}
