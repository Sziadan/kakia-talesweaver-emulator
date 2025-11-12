

using kakia_talesweaver_packets.Models;
using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class MovementRequestPacket
{
	public byte Flag1 { get; set; }
	public TsPoint Position { get; set; } = new ();
	public byte Direction { get; set; }

	public static MovementRequestPacket? FromBytes(byte[] bytes)
	{
		using PacketReader pr = new (bytes);
		pr.Skip(1); // Packet ID

		byte flag1 = pr.ReadByte();

		
		if (flag1 != 0x00 || bytes.Length != 7)
		{
			return null; // Invalid movement request
		}
		

		var packet = new MovementRequestPacket
		{
			Flag1 = pr.ReadByte(),
			Position = new TsPoint
			{
				X = pr.ReadUInt16BE(),
				Y = pr.ReadUInt16BE()				
			},
			Direction = 0//pr.ReadByte()
		};
		return packet;
	}
}
