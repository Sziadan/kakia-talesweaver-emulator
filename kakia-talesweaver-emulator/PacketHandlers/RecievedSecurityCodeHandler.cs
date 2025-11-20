using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_logging;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_packets.Packets;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.InputtedSecurityCode)]
public class RecievedSecurityCodeHandler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var csp = ClientRespondPacket.FromBytes(p.Data);
		if (csp.ResponseCode != ClientRespondCode.WithMessage)
		{
			Logger.Log($"Client responded with code: {csp.ResponseCode}");
			return;
		}

		// Code is in csp.Message, handle it later
		var session = client.GetSessionInfo();
		client.RemoveSessionInfo();

		if (session == null)
		{
			Logger.Log("Session info not found for client after security code input.");
			return;
		}

		uint seed = (uint)Random.Shared.Next();
		var pClient = (client as PlayerClient)!;
		pClient._server.AccountSessions.AddOrUpdate(seed, session, (_, _) => session);

		ServerConnectPacket scp = new()
		{			
			IP = System.Net.IPAddress.Parse(TalesServer.serverIp),
			Port = 20002,
			Flag1 = 2,
			Seed = seed,
			Username = session.AccountId,
			Unk = 0x27,
			Flag2 = 0
		};

		client.Send(scp.ToBytes(), CancellationToken.None).Wait();
	}
}
