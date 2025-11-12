using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class CharLoginSecurityPacket
{
	public CharLoginSecurityType charLoginSecurityType { get; set; } = CharLoginSecurityType.None;

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write((byte)0x3C); // Header
		pw.Write((byte)charLoginSecurityType);
		return pw.ToArray();
	}
}

public enum CharLoginSecurityType : byte
{
	CodeUpdated = 0x00,
	None = 0x01,
	SetupSecurityCode = 0x02,
	RequestSecurityCode = 0x03
}
