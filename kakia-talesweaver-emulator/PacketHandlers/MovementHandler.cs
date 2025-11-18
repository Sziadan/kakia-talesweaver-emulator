using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_logging;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_packets.Packets;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.Movement)]
public class MovementHandler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var movement = MovementRequestPacket.FromBytes(p.Data);
		if (movement.Flag == MovementFlag.MovementStop)
		{
			// Not movement
			return;
		}

		var character = client.GetCharacter();

		if (movement.Flag == MovementFlag.InitialRequest)
		{
			var movePacket = new MoveObjectPacket
			{
				ObjectID = character.Id,
				MoveType = 1,
				MoveSpeed = 27,
				PreviousPosition = character.Position,
				TargetPosition = movement.Position,
				Direction = movement.Direction
			};
			client.Broadcast(movePacket.ToBytes());
		}
		character.Position = movement.Position;

		// temporary fix for spawn position desync after movement
		character.SpawnCharacterPacket.Movement.XPos = movement.Position.X;
		character.SpawnCharacterPacket.Movement.YPos = movement.Position.Y;

		if (movement.Flag == MovementFlag.InitialRequest) return;

		var map = client.GetCurrentMap();
		if (map is not null)
		{
			var portal = map.EnteredPortal(movement.Position);
			if (portal is not null)
			{
				Logger.Log($"Touched portal ({portal.Id}) at {portal.MinPoint.X}:{portal.MinPoint.Y}", LogLevel.Information);
				string destKey = $"{portal.Destination.MapId}-{portal.Destination.ZoneId}";

				client.LoadMap(TalesServer.Maps[destKey], true, portal.Destination.Position);


				/*
				switch (portal.Id)
				{
					case 4046323968:
						client.LoadMap(TalesServer.Maps["3-19200"], true, CancellationToken.None);
						break;

					case 1764753664: // class room
						client.LoadMap(TalesServer.Maps["6-38144"], true, CancellationToken.None);
						break;

					case 1275396352:
						client.LoadMap(TalesServer.Maps["6-38656"], true, CancellationToken.None);
						break;


					case 1059258624:
						client.LoadMap(TalesServer.Maps["6-38656"], true, CancellationToken.None);
						break;

					default:
						break;
				}
				*/
			}
		}
	}
}
