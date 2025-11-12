using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class LoginPacket
{
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;

	public static LoginPacket? FromBytes(byte[] b)
	{
		using PacketReader pr = new(b);
		pr.Skip(1);
		var flag = pr.ReadByte();
		if (flag != 0)
			return null;
		return new LoginPacket()
		{
			Username = pr.ReadPrefixedString(),
			Password = pr.ReadPrefixedString()
		};
	}
}
