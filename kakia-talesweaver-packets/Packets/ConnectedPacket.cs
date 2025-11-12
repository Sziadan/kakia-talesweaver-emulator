using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;
public class ConnectedPacket
{
	public byte Index { get; init; } = 0x7E;
	public byte Id { get; init; } = 0x1B;
	public string Message { get; init; } = "CONNECTED SERVER\n";

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write(Index);
		pw.Write(Id);
		pw.WriteRawASCII(Message);
		return pw.ToArray();
	}
}
