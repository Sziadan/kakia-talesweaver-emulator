using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Models;

public class WarpPortal
{
	public uint Id { get; set; }
	public TsPoint MinPoint { get; set; } = new();
	public TsPoint MaxPoint { get; set; } = new();
	public WarpDestination Destination { get; set; } = new();

	public static WarpPortal FromBytes(byte[] b)
	{
		using PacketReader pr = new(b);
		pr.Skip(3);
		return new()
		{
			Id = pr.ReadUInt32BE(),
			MinPoint = new TsPoint
			{
				X = pr.ReadUInt16BE(),
				Y = pr.ReadUInt16BE()
			},
			MaxPoint = new TsPoint
			{
				X = pr.ReadUInt16BE(),
				Y = pr.ReadUInt16BE()
			}
		};
	}

	public bool CollidedWith(TsPoint characterPosition)
	{
		if (characterPosition == null)
			return false;

		int cx = characterPosition.X;
		int cy = characterPosition.Y;

		return cx >= MinPoint.X && cx <= MaxPoint.X && cy >= MinPoint.Y && cy <= MaxPoint.Y;
	}
}
