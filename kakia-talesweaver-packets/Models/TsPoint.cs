using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Models;

public class TsPoint
{
	public ushort X { get; set; }
	public ushort Y { get; set; }

	public TsPoint() { }
	public TsPoint(ushort x, ushort y)
	{
		X = x;
		Y = y;
	}

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write(X);
		pw.Write(Y);
		return pw.ToArray();
	}
}
