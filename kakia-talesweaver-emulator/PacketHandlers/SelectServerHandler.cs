using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_logging;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_packets.Packets;
using kakia_talesweaver_utils;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.ServerSelect)]
public class SelectServerHandler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		Logger.Log("SELECT SERVER RECV");

		using PacketReader pr = new(p.Data);
		pr.Skip(3);
		string accountName = pr.ReadPrefixedString();
		Logger.Log($"Logging in as: {accountName}");

		uint seed = (uint)Random.Shared.Next();
		(client as PlayerClient)!._server.AccountSessions.AddOrUpdate(seed, accountName, (_, _) => accountName);

		ServerConnectPacket scp = new()
		{
			Username = accountName,
			IP = System.Net.IPAddress.Parse(TalesServer.serverIp),
			Port = 20001,
			Flag1 = 2,
			Unk = 0x2D,
			Flag2 = 0,
			Seed = seed
		};

		client.Send(scp.ToBytes(), CancellationToken.None).Wait();
	}
}
