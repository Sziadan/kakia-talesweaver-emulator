using kakia_talesweaver_emulator.Models;
using kakia_talesweaver_network;
using kakia_talesweaver_packets.Packets;
using System.Collections.Concurrent;

namespace kakia_talesweaver_emulator.Network;

public class TalesServer : SocketServer
{
	public static string serverIp = "127.0.0.1";

	public ConcurrentDictionary<string, IPlayerClient> ConnectedPlayers { get; } = new();
	public ConcurrentDictionary<uint, string> AccountSessions { get; set; }
	private ServerType _serverType = ServerType.Login;
	public static Dictionary<int, List<byte[]>> NPCs = new()
	{
		{ 1, new List<byte[]>() }
	};

	public static ServerListPacket ServerList { get; } = new()
	{
		Servers = new()
		{
			new ServerInfo()
			{
				Id = 26,
				IP = System.Net.IPAddress.Parse(TalesServer.serverIp),
				Port = 20000,
				Name = "Kakia Tales Server",
				Load = 8
			}
		}
	};

	public TalesServer(string host, int port, ServerType serverType, ConcurrentDictionary<uint, string> accountSessions) : base(host, port)
	{
		_serverType = serverType;
		AccountSessions = accountSessions;

		var npcFiles = Directory.GetFiles("NPCs", "*.bin");
		foreach (var npcFile in npcFiles)
		{
			var npcData = File.ReadAllBytes(npcFile);
			NPCs[1].Add(npcData);
		}
	}

	public override void OnConnect(SocketClient s)
	{
		var pc = new PlayerClient(s, _serverType, this);
		if (_serverType == ServerType.Login) return;

		ConnectedPlayers.AddOrUpdate(s.GetIP(), pc, (_, _) => pc);
	}

	public void Broadcast(IPlayerClient sender, byte[] data, bool includeSelf, CancellationToken ct)
	{
		foreach (var player in ConnectedPlayers.Values)
		{
			if ((!includeSelf && player == sender) || player.GetServerType() != ServerType.World)
				continue;

			player.Send(data, ct).Wait(ct);
		}
	}
}
