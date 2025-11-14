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
		//Logger.Log($"Recieved packetType [{packet.PacketId.ToString()}]", LogLevel.Debug);
		//Logger.Log($"PckData: {Environment.NewLine} {packet.Data.ToFormatedHexString()}", LogLevel.Debug);
		if (handler != null)
		{
			try
			{
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

	public void LoadMap(MapInfo map, bool sendEffect, CancellationToken ct)
	{
		Broadcast(new SendCharEffectPacket()
		{
			ObjectId = Character.Id,
			Effect = CharEffect.TeleportEffect2
		}.ToBytes(), includeSelf: true);
		Task.Delay(2000, ct).Wait(ct);

		MapId = map.MapId;
		ZoneId = map.ZoneId;

		Send(map.GetMapPacket().ToBytes(), ct).Wait(ct);
		Character.Position = map.SpawnPoints[0].Position.Copy();
		Character.Direction = map.SpawnPoints[0].Direction;

		Character.SpawnCharacterPacket.Movement.XPos = Character.Position.X;
		Character.SpawnCharacterPacket.Movement.YPos = Character.Position.Y;
		Character.SpawnCharacterPacket.Movement.MovePathData[0] = Character.Direction;

		Send(Character.SpawnCharacterPacket.ToBytes(), ct).Wait(ct);
		Broadcast(Character.SpawnCharacterPacket.ToBytes(SetAsOther: true), false);
		Send(new InitObjectIdPacket(Character.Id).ToBytes(), ct).Wait(ct);


		Send(@"4D 00 00 01 01 00 2D CF 94 00 01 00 01 01 00 00
00 00 00 00 00 00 00 00 00 00 00 ".ToByteArray(), CancellationToken.None).Wait();


		foreach (var subList in map.Entities)
		{
			foreach (var entity in subList.Value)
			{
				if (entity[2] == 0x04)
					Send(entity, ct).Wait(ct);
			}
		}


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
