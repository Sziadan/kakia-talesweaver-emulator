using System.IO.Compression;
using System.Net;
using System.Text;

namespace kakia_talesweaver_utils;

public class PacketWriter : BinaryWriter
{
	public PacketWriter() : base(new MemoryStream())
	{
	}

	public void Write(IPAddress ipaddr)
	{
		Write(ipaddr.GetAddressBytes().Reverse().ToArray());
	}

	public void WriteCompressedShiftJISString(string str)
	{
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		var bytes = Encoding.GetEncoding("shift_jis").GetBytes(str);

		using var inMs = new MemoryStream(bytes);
		using var outMs = new MemoryStream();
		using (var z = new ZLibStream(outMs, CompressionMode.Compress, leaveOpen: true))
		{
			inMs.CopyTo(z);
		}

		var compressed = outMs.ToArray();

		Write((byte)0x01);
		Write((ushort)compressed.Length);
		Write(compressed);
	}

	public void WriteRawASCII(string str)
	{
		foreach (var c in str)
			Write((byte)c);
	}

	public void WritePacket(byte[] packet)
	{
		ushort len = (ushort)packet.Length;
		Write((byte)0xAA);
		Write(len);
		Write(packet);
	}

	public void Align()
	{
		while (BaseStream.Position % 4 != 0)
			Write((byte)0x00);
	}

	public byte[] ToArray()
	{
		return ((MemoryStream)BaseStream).ToArray();
	}

	#region Deal with endianness
	public override void Write(short value)
	{
		var bytes = BitConverter.GetBytes(value);
		if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
		base.Write(bytes);
	}

	public override void Write(ushort value)
	{
		var bytes = BitConverter.GetBytes(value);
		if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
		base.Write(bytes);
	}

	public override void Write(int value)
	{
		var bytes = BitConverter.GetBytes(value);
		if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
		base.Write(bytes);
	}

	public override void Write(uint value)
	{
		var bytes = BitConverter.GetBytes(value);
		if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
		base.Write(bytes);
	}

	public override void Write(long value)
	{
		var bytes = BitConverter.GetBytes(value);
		if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
		base.Write(bytes);
	}

	public override void Write(ulong value)
	{
		var bytes = BitConverter.GetBytes(value);
		if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
		base.Write(bytes);
	}

	public override void Write(float value) => Write(BitConverter.SingleToInt32Bits(value));
	public override void Write(double value) => Write(BitConverter.DoubleToInt64Bits(value));
	#endregion
}
