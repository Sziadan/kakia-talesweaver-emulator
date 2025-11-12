using kakia_talesweaver_utils;
using System.Net;

namespace kakia_talesweaver_packets.Packets;

public class ServerConnectPacket
{
	public IPAddress IP { get; set; } = IPAddress.Loopback;
	public ushort Port { get; set; }
	public byte Flag1 { get; set; }
	public uint Seed { get; set; }
	public string Username { get; set; } = string.Empty;
	public byte Unk { get; set; }
	public byte Flag2 { get; set; }

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write((byte)0x03); // Header
		pw.Write(IP);
		pw.Write(Port);
		pw.Write(Flag1);
		pw.Write(Seed);
		pw.Write(Username);
		pw.Write(Unk);
		pw.Write(Flag2);
		return pw.ToArray();
	}
}
