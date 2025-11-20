using kakia_talesweaver_emulator.Models;
using kakia_talesweaver_network;
using kakia_talesweaver_packets.Models;
using kakia_talesweaver_packets.Packets;
using System.Collections.Concurrent;

namespace kakia_talesweaver_emulator.Network;

public class TalesServer : SocketServer
{
	public static string serverIp = "127.0.0.1";

	public ConcurrentDictionary<string, IPlayerClient> ConnectedPlayers { get; } = new();
	public ConcurrentDictionary<uint, SessionInfo> AccountSessions { get; set; }
	private ServerType _serverType = ServerType.Login;
	public static Dictionary<string, MapInfo> Maps = new();

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

	public TalesServer(string host, int port, ServerType serverType, ConcurrentDictionary<uint, SessionInfo> accountSessions) : base(host, port)
	{
		_serverType = serverType;
		AccountSessions = accountSessions;

		var maps = Directory.GetDirectories("Maps");


		foreach (var map in maps)
		{
			var zones = Directory.GetDirectories(map);
			foreach (var zone in zones)
		{
				var mapInfo = new MapInfo(zone);
				string key = $"{mapInfo.MapId}-{mapInfo.ZoneId}";

				if (!Maps.ContainsKey(key))
					Maps.Add(key, mapInfo);
			}
		}
	}

	public override void OnConnect(SocketClient s)
	{
		var pc = new PlayerClient(s, _serverType, this);
		if (_serverType == ServerType.Login) return;

		ConnectedPlayers.AddOrUpdate(Guid.NewGuid().ToString(), pc, (_, _) => pc);
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
