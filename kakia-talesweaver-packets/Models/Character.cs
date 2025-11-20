using kakia_talesweaver_utils;
using kakia_talesweaver_utils.Extensions;

namespace kakia_talesweaver_packets.Models;

public class Character
{	
	public uint LastLoginTime { get; set; }
	public uint CreationTime { get; set; }	
	public uint ModelId { get; set; }
	public uint Id { get; set; }
	public string Name { get; set; } = string.Empty;

	public ObjectPos Position { get; set; } = new ObjectPos();
	public ushort MapId { get; set; }
	public ushort ZoneId { get; set; }

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write(LastLoginTime);
		pw.Write(CreationTime);

		// UNKOWN
		pw.Write(0);
		pw.Write((byte)0);

		pw.Write(ModelId);

		
		// UNKOWN Gear, appearance etc
		string unkGear = string.Empty;
		if (Id != 0)
		{
			unkGear = @"00 01 00 00 00 00 00 0E 00 0F A0 32 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 0F A1 6E 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 0F A1 E7 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 FF 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00";
		}
		else
		{
			unkGear = @"00 01 00 00 00 00 00 00 FF 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
						00 00 00 00 00 00 00 00";
		}
		pw.Write(unkGear.ToByteArray());


		pw.Write(Id);
		pw.Write((byte)Name.Length);
		pw.WriteRawASCII(Name);
		
		return pw.ToArray();
	}
}
