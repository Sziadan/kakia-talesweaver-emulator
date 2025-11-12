using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class ChatPacket
{
	public uint CharacterId { get; set; }
	public string Message { get; set; } = string.Empty;

	public static ChatPacket FromBytes(byte[] data)
	{
		using PacketReader pr = new(data);
		byte packetType = pr.ReadByte();
		pr.Skip(1); // Skip the packet sub-type byte

		var packet = new ChatPacket();
		if (packetType == 0x0D)
			packet.CharacterId = pr.ReadUInt32();		
		packet.Message = pr.ReadPrefixedString();

		return packet;
	}

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write((byte)0x0D); // Packet type
		pw.Write((byte)0x00); // Packet sub-type
		pw.Write(CharacterId);
		pw.Write((byte)Message.Length);
		pw.WriteRawASCII(Message);
		pw.Write('\0');
		return pw.ToArray();
	}
}
