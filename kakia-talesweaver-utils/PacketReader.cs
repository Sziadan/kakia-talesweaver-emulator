using System.Text;

namespace kakia_talesweaver_utils;

public class PacketReader : BinaryReader
{
	public PacketReader(Stream input) : base(input)
	{
	}

	public PacketReader(byte[] input) : base(new MemoryStream(input))
	{
	}

	public ulong ReadUInt64BE()
	{
		var bytes = ReadBytes(8);
		if (bytes.Length < 8)
		{
			throw new EndOfStreamException();
		}
		Array.Reverse(bytes);
		return BitConverter.ToUInt64(bytes, 0);
	}

	public long ReadInt64BE()
	{
		var bytes = ReadBytes(8);
		if (bytes.Length < 8)
		{
			throw new EndOfStreamException();
		}
		Array.Reverse(bytes);
		return BitConverter.ToInt64(bytes, 0);
	}

	public uint ReadUInt32BE()
	{
		var bytes = ReadBytes(4);
		if (bytes.Length < 4)
		{
			throw new EndOfStreamException();
		}
		Array.Reverse(bytes);
		return BitConverter.ToUInt32(bytes, 0);
	}

	public int ReadInt32BE()
	{
		var bytes = ReadBytes(4);
		if (bytes.Length < 4)
		{
			throw new EndOfStreamException();
		}
		Array.Reverse(bytes);
		return BitConverter.ToInt32(bytes, 0);
	}

	public short ReadInt16BE()
	{
		var bytes = ReadBytes(2);
		if (bytes.Length < 2)
		{
			throw new EndOfStreamException();
		}
		Array.Reverse(bytes);
		return BitConverter.ToInt16(bytes, 0);
	}

	public ushort ReadUInt16BE()
	{
		var bytes = ReadBytes(2);
		if (bytes.Length < 2)
		{
			throw new EndOfStreamException();
		}
		Array.Reverse(bytes);
		return BitConverter.ToUInt16(bytes, 0);
	}

	public string ReadPrefixedString()
	{
		int length = ReadByte();
		return Encoding.ASCII.GetString(ReadBytes(length));
	}

	public void Skip(int count)
	{
		BaseStream.Seek(count, SeekOrigin.Current);
	}
}
