using kakia_talesweaver_utils;
using System.Net;

namespace kakia_talesweaver_packets.Packets;

public class KeySeedPacket
{
	public byte Index { get; init; } = 0x00;
	public byte Id { get; init; } = 0x00;
	public byte SubFlag { get; set; } = 0x02;
	public uint Seed { get; init; }
	public IPAddress IPAddress { get; set; }
	public string ShiftJISMOTD { get; set; } = "Welcome to Kakia TalesWeaver Server!";
	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write(Index);
		pw.Write(Id);
		pw.Write(SubFlag);
		pw.Write(Seed);
		pw.Write(IPAddress);
		pw.WriteCompressedShiftJISString(ShiftJISMOTD);
		return pw.ToArray();
	}
}
