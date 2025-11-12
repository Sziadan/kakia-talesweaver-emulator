using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class SkillEntry
{
	public int SkillID { get; set; }
	public ulong ExpiryTime { get; set; }
	public int SkillLevel { get; set; }
	public byte Flag1 { get; set; }
	public int Unknown1 { get; set; }
	public short Unknown2 { get; set; }
	public byte Flag2 { get; set; }

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write(SkillID);
		pw.Write(ExpiryTime);
		pw.Write(SkillLevel);
		pw.Write(Flag1);
		pw.Write(Unknown1);
		pw.Write(Unknown2);
		pw.Write(Flag2);
		return pw.ToArray();
	}
}
