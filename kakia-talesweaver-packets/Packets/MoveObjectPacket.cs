using kakia_talesweaver_packets.Models;
using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class MoveObjectPacket
{
	public uint ObjectID { get; set; }
	public MovementType MoveType { get; set; }
	public byte MoveSpeed { get; set; }
	public TsPoint PreviousPosition { get; set; } = new();
	public TsPoint TargetPosition { get; set; } = new();
	public byte Direction { get; set; }

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write((byte)0x0B); // Packet ID
		pw.Write((byte)0); // Sub ID
		pw.Write(ObjectID);
		pw.Write((byte)MoveType);
		pw.Write(MoveSpeed);
		pw.Write(PreviousPosition.ToBytes());
		pw.Write(TargetPosition.ToBytes());
		pw.Write(Direction);
		return pw.ToArray();
	}
}
