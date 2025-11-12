
using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class CharacterListPacket
{
	public List<CharacterInfo> Characters { get; set; } = new();

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write((byte)0x50); // Header
		pw.Write((byte)0x01); // Header
		pw.Write((byte)Characters.Count);
		foreach (var character in Characters)
		{
			pw.Write(character.ToBytes());
		}
		return pw.ToArray();
	}
}


public class CharacterInfo
{
	public byte ServerId { get; set; }
	public string Name { get; set; } = string.Empty;
	public int unk1 { get; set; } = 0;
	public int time1 { get; set; } = 0x661da586;
	public int time2 { get; set; } = 0x66d38b6c;
	public byte unk_flag { get; set; } = 0x01;

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write(ServerId);
		pw.Write((byte)Name.Length);
		pw.WriteRawASCII(Name);
		pw.Write((byte)0x00); // Name terminator
		pw.Write(unk1);
		pw.Write(time1);
		pw.Write(time2);
		pw.Write(unk_flag);
		return pw.ToArray();
	}
}