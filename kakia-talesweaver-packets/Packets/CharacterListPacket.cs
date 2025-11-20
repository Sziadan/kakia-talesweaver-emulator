
using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class CharacterListPacket
{
	public List<AccountIdInfo> AccountId { get; set; } = new();

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write((byte)0x50); // Header
		pw.Write((byte)0x01); // Header
		pw.Write((byte)AccountId.Count);
		foreach (var character in AccountId)
		{
			pw.Write(character.ToBytes());
		}
		return pw.ToArray();
	}
}


public class AccountIdInfo
{
	public byte ServerId { get; set; }
	public string Name { get; set; } = string.Empty;
	public int Unk1 { get; set; }
	public int Time1 { get; set; } = 0x661da586;
	public int Time2 { get; set; } = 0x66d38b6c;
	public byte CharacterCount { get; set; }

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write(ServerId);
		pw.Write((byte)Name.Length);
		pw.WriteRawASCII(Name);
		pw.Write((byte)0x00); // Name terminator
		pw.Write(Unk1);
		pw.Write(Time1);
		pw.Write(Time2);
		pw.Write(CharacterCount);
		return pw.ToArray();
	}
}