

using kakia_talesweaver_packets.Models;
using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class MovementRequestPacket
{
	public MovementFlag Flag { get; set; }
	public MovementType MoveType { get; set; }
	public TsPoint Position { get; set; } = new ();
	public byte Direction { get; set; }

	public static MovementRequestPacket? FromBytes(byte[] bytes)
	{
		using PacketReader pr = new (bytes);
		pr.Skip(1); // Packet ID


		var packet = new MovementRequestPacket()
		{ 
			Flag = (MovementFlag)pr.ReadByte()
		};

		switch (packet.Flag)
		{
			case MovementFlag.InitialRequest:
				packet.MoveType = (MovementType)pr.ReadByte();
				packet.Position = new TsPoint(pr.ReadUInt16BE(), pr.ReadUInt16BE());
				//packet.Direction = pr.ReadByte();
				break;

			case MovementFlag.CurrentLocation:
				packet.Flag = MovementFlag.CurrentLocation;
				packet.Position = new TsPoint(pr.ReadUInt16BE(), pr.ReadUInt16BE());
				break;

			case MovementFlag.MovementStop:
			default:
				break;
		}

		return packet;
	}
}

public enum MovementFlag : byte
{
	InitialRequest = 0x00,
	MovementStop = 0x01,
	CurrentLocation = 0x02
}

public enum MovementType : byte
{
	Walk = 0x00,
	Run = 0x01,
	Unknown
}