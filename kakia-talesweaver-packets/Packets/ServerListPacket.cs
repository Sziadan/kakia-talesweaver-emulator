using kakia_talesweaver_utils;
using System.Net;

namespace kakia_talesweaver_packets.Packets;

public class ServerListPacket
{
	public List<ServerInfo> Servers { get; set; } = new();

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write((byte)0x56); // Header
		pw.Write((byte)Servers.Count);
		foreach (var server in Servers)
		{
			pw.Write(server.ToBytes());
		}
		pw.Align();
		return pw.ToArray();
	}
}

public class  ServerInfo
{
	public byte Id { get; set; }
	public IPAddress IP { get; set; }
	public ushort Port { get; set; }
	public string Name { get; set; }
	public ushort Load { get; set; }

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write(Id);
		pw.Write(IP.GetAddressBytes().Reverse().ToArray());
		pw.Write(Port);
		pw.Write(Name);
		pw.Write((byte)0); // Null terminator for the server name	
		pw.Write(Load);
		return pw.ToArray();
	}
}
