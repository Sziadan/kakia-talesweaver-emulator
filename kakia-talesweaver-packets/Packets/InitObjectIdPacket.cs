using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class InitObjectIdPacket
{
	public uint ObjectId { get; set; }
	public InitObjectIdPacket(uint objectId)
	{
		ObjectId = objectId;
	}
	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write(0x33);
		pw.Write((byte)0x01); // Sub-type, 0x01 for InitObjectId
		pw.Write(ObjectId);
		pw.Write((byte)0x00); // Padding byte
		return pw.ToArray();
	}
}
