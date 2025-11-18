using kakia_talesweaver_emulator.Models;
using kakia_talesweaver_emulator.PacketHandlers;
using kakia_talesweaver_logging;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_packets.Models;
using kakia_talesweaver_packets.Packets;
using kakia_talesweaver_utils.Extensions;
using System;
using System.Net.Sockets;

namespace kakia_talesweaver_emulator.Network;

public class PlayerClient : IPlayerClient
{
	public TalesServer _server;
	private SocketClient _socketClient;
	private ServerType _serverType;
	private bool _cryptoSet = false;

	public string AccountName = string.Empty;
	public ushort MapId = 0;
	public ushort ZoneId = 0;
	public PlayerCharacter Character = new();

	public PlayerClient(SocketClient socketClient, ServerType serverType, TalesServer server)
	{
		_socketClient = socketClient;
		_socketClient.PacketReceived += this.PacketRecieved;
		_serverType = serverType;
		_server = server;

		Logger.Log($"Player connected (to server: {serverType}): {_socketClient.GetIP()}", LogLevel.Information);

		_ = Send(new ConnectedPacket().ToBytes(), CancellationToken.None);
		_ = _socketClient.BeginRead();

		if (serverType == ServerType.Login)
		{
			_socketClient.SetCrypto();
			_ = Send(TalesServer.ServerList.ToBytes(), CancellationToken.None);
		}
	}

	public ServerType GetServerType()
	{
		return _serverType;
	}

	public async Task PacketRecieved(RawPacket packet)
	{
		if (!_cryptoSet && _serverType == ServerType.Login && packet.Data.Length == 5)
		{
			_cryptoSet = true;
			return;
		}

		PacketHandler handler = PacketHandlers.PacketHandlers.GetHandlerFor((PacketType)packet.PacketId);
		if (handler != null)
		{
			try
			{
				Logger.Log($"Recieved packetType [{packet.PacketId.ToString()}]", LogLevel.Debug);
				Logger.Log($"PckData: {Environment.NewLine}{packet.Data.ToFormatedHexString()}", LogLevel.Debug);

				handler.HandlePacket(this, packet);
				return;
			}
			catch (Exception e)
			{
				Logger.Log(e);
			}
		}
		else
		{
			Logger.Log($"NOT IMPLEMENTED [{packet.PacketId}]", LogLevel.Warning);
			Logger.LogPck(packet.Data);
		}
		

		//Logger.Log($"Recieved packet of length {packet.Data.Length}: {Environment.NewLine}{packet.Data.ToFormatedHexString()}", LogLevel.Debug);
	}

	public async Task<bool> Send(byte[] packet, CancellationToken token)
	{
		//Logger.Log($"Sending [{((PacketType)BitConverter.ToUInt16(packet, 0))}]", LogLevel.Debug);
		await _socketClient!.Send(packet);
		return true;
	}

	public void UpdateCryptoSeed(uint seed)
	{
		_server.AccountSessions.TryGetValue(seed, out var session);
		if (session != null)
		{
			AccountName = session;
			Logger.Log($"Account {AccountName} assigned to session {seed}", LogLevel.Information);
		}

		_socketClient!.SetCrypto(seed);
	}

	public void Broadcast(byte[] packet, bool includeSelf = true)
	{
		_server.Broadcast(this, packet, includeSelf, CancellationToken.None);
	}

	public PlayerCharacter GetCharacter()
	{
		return Character;
	}

	public void SetCharacter(string? name = null)
	{
		string path = Path.Combine("Accounts", string.IsNullOrEmpty(name) ? AccountName : name);
		Character.SpawnCharacterPacket = SpawnCharacterPacket.FromBytes(
			File.ReadAllText($"{path}.txt")
			.ToByteArray());

		Character.Id = Character.SpawnCharacterPacket.UserId;
		Character.Name = Character.SpawnCharacterPacket.UserName;
		Character.Position = new TsPoint(Character.SpawnCharacterPacket.Movement.XPos, Character.SpawnCharacterPacket.Movement.YPos);
	}

	public TalesServer GetServer()
	{
		return _server;
	}

	public void LoadMap(MapInfo map, bool sendEffect, ObjectPos? spawnPos = null, CancellationToken ct = default)
	{
		if (sendEffect)
		{
			Broadcast(new SendCharEffectPacket()
			{
				ObjectId = Character.Id,
				Effect = CharEffect.TeleportEffect2
			}.ToBytes(), includeSelf: true);
			Task.Delay(1500, ct).Wait(ct);
		}

		MapId = map.MapId;
		ZoneId = map.ZoneId;

		Send(map.GetMapPacket().ToBytes(), ct).Wait(ct);

		if (spawnPos != null)
		{
			Character.Position = spawnPos.Position.Copy();
			Character.Direction = spawnPos.Direction;
		}
		else
		{
			Character.Position = map.SpawnPoints[0].Position.Copy();
			Character.Direction = map.SpawnPoints[0].Direction;
		}

		Character.SpawnCharacterPacket.Movement.XPos = Character.Position.X;
		Character.SpawnCharacterPacket.Movement.YPos = Character.Position.Y;
		Character.SpawnCharacterPacket.Movement.MovePathData[0] = Character.Direction;

		Send(Character.SpawnCharacterPacket.ToBytes(), ct).Wait(ct);
		Broadcast(Character.SpawnCharacterPacket.ToBytes(SetAsOther: true), false);
		Send(new InitObjectIdPacket(Character.Id).ToBytes(), ct).Wait(ct);


		// Send Gear and stats etc

		// buffs
		Send(@"2C 03 C6 85 96 00 05 03 C6 85 D5 2B 00 2D E1 94 
00 01 00 00 03 C6 85 D6 2B 00 2D E1 95 00 01 00 
00 03 C6 85 D7 2B 00 2D E1 96 00 01 00 00 03 C6 
85 D8 2B 00 2D E3 D6 00 01 00 00 03 C6 85 D9 2B 
00 2D E7 4A 00 01 00 00 00".ToByteArray(), CancellationToken.None).Wait();

		// Stats
		Send(@"08 00 00 FD FF 00 1E 84 8D 00 00 00 00 00 00 01 
00 02 01 00 00 00 00 00 00 00 00 01 27 00 00 00 
00 00 00 01 27 00 00 00 00 00 00 00 49 00 00 00 
00 00 00 00 49 00 00 00 00 00 00 04 C5 00 00 00 
00 00 00 04 C5 00 00 00 00 00 00 00 00 00 00 00 
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
00 00 00 00 00 00 02 00 02 00 00 00 00 00 00 00 
CD 00 00 00 00 00 00 00 64 00 00 00 00 00 00 00 
FA 00 01 00 00 00 00 00 00 27 10 00 00 00 00 00 
00 01 27 00 00 00 00 00 00 00 00 49 00 00 00 00 
00 00 04 C5 00 00 00 00 00 00 00 CD 00 00 00 00 
00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 04 
00 01 00 03 00 03 00 03 00 03 00 02 00 04 00 01 
00 03 00 03 00 03 00 03 00 01 00 00 00 00 00 00 
00 00 00 00 00 00 00 00 00 00 00 00 05 00 19 00 
02 00 08 00 00 00 02 00 01 00 01 00 00 00 00 16 
1B 00 00 02 6A 00 00 04 D6 00 00 02 67 00 00 04 
D0 00 00 00 39 00 00 00 15 59 12 12 00 05 00 05 
00 05 00 05 00 0A 00 05 00 05 00 0A 00 00 00 00 
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 BA 
7D EF 30 00 00 00 00 00 00 98 96 80 00 00 00 00 
00 00 00 00 00".ToByteArray(), CancellationToken.None).Wait();

		// Gear
		Send(@"37 00 0F A6 E4 03 C6 85 B1 0A 81 F5 83 5B 83 62 
83 77 83 8B 00 00 00 00 00 00 00 00 00 00 00 00 
00 00 00 00 00 00 08 01 00 00 01 00 00 00 01 01 
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
00 08 00 0A 00 0D 00 04 00 18 00 00 00 00 00 02 
00 01 00 01 00 01 00 01 04 00 5F FF 00 00 00 00 
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
00 00 00 00 00 00 00 00 FF 00".ToByteArray(), CancellationToken.None).Wait();


		Send(@"4D 00 00 01 01 00 2D CF 94 00 01 00 01 01 00 00
00 00 00 00 00 00 00 00 00 00 00 ".ToByteArray(), CancellationToken.None).Wait();


		foreach (var subList in map.Entities)
		{
			foreach (var entity in subList.Value)
			{
				// Do not include items
				if (entity[2] == 0x03)
					continue;

				// Do not include pets
				if (entity[2] == 0x06)
					continue;

				Send(entity, ct).Wait(ct);
			}
		}

		/*
		foreach (var player in _server.ConnectedPlayers.Values)
		{
			if (player == this || string.IsNullOrEmpty(player.GetCharacter().Name))
				continue;
			if (!player.InMapZone(MapId, ZoneId))
				continue;

			Send(player.GetCharacter()
				.SpawnCharacterPacket
				.ToBytes(SetAsOther: true), ct).Wait(ct);
		}
		*/
	}

	public bool InMapZone(ushort mapId, ushort zoneId)
	{
		return MapId == mapId && ZoneId == zoneId;
	}

	public MapInfo? GetCurrentMap()
	{
		string key = $"{MapId}-{ZoneId}";
		TalesServer.Maps.TryGetValue(key, out var map);
		return map;
	}
}
