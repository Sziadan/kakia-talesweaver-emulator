using kakia_talesweaver_packets.Models;
using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class CharacterSelectList
{
	public List<Character> Characters { get; set; } = new();
	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write((byte)0x6B); // Header
		pw.Write((byte)(Characters.Any() ? 1 : 0)); // Has Characters ??
		pw.Write((byte)Characters.Count);
		pw.Write((byte)1); // Character index

		if (Characters.Count == 0)
		{
			pw.Write((short)0);
			return pw.ToArray();
		}

		for (int i = 0; i < Characters.Count; i++)
		{
			pw.Write(Characters[i].ToBytes());

			if (i < Characters.Count - 1)
			{
				pw.Write((byte)(i == 0 ? 0 : i + 1)); // Separator
			}
		}
		return pw.ToArray();
	}
}
