using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class ClientRespondPacket
{
	public ClientRespondCode ResponseCode { get; set; }
	public string Message { get; set; } = string.Empty;

	public static ClientRespondPacket FromBytes(byte[] data)
	{
		using PacketReader pr = new(data);
		pr.Skip(1);
		ClientRespondPacket packet = new()
		{
			ResponseCode = (ClientRespondCode)pr.ReadByte()			
		};

		if (packet.ResponseCode == ClientRespondCode.WithMessage)
			packet.Message = pr.ReadPrefixedString();

		return packet;
	}
}

public enum ClientRespondCode : byte
{
	Unk = 0x00,
	WithMessage = 0x01,
	Unk1 = 0x02,
	Unk2 = 0x03,
	Unk3 = 0x04,
	Unk4 = 0x05 // Loading done??
}
