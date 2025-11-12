using kakia_talesweaver_emulator.Network;
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
		if (movement is null)
		{
			// Not movement
			return;
		}

		var character = client.GetCharacter();
		var movePacket = new MoveObjectPacket
		{
			ObjectID = character.Id,
			MoveType = 1,
			MoveSpeed = 27,
			PreviousPosition = character.Position,
			TargetPosition = movement.Position,
			Direction = movement.Direction
		};
		character.Position = movement.Position;
		client.Broadcast(movePacket.ToBytes());
	}
}
