using kakia_talesweaver_network.Cryptography;
using System.Buffers.Binary;

namespace kakia_talesweaver_network;

public class RawPacket
{
	public byte[] Data { get; }
	public byte PacketId => Data[0] == 0xAA ? Data[3] : Data[0];

	public RawPacket(byte[] data)
	{
		Data = data;
	}

	public static List<RawPacket> ParsePackets(byte[] buffer, CryptoHandler cryptoHandler)
	{
		List<RawPacket> packets = new();
		int position = 0;
		while (position < buffer.Length)
		{
			if (buffer[position] != 0xAA)
				break;

			if (buffer.Length - position < 3)
				break;

			ushort length = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan().Slice(1,2));
			if (buffer.Length - position < length + 3)
				break;

			byte[] packetData = new byte[length + 3];
			Array.Copy(buffer, position, packetData, 0, length + 3);

			if (cryptoHandler.IsEncrypted)
			{
				byte[]? decryptedData = cryptoHandler.Decrypt(packetData);
				if (decryptedData == null)
					break;
				packetData = decryptedData;
			}

			packets.Add(new RawPacket(packetData));
			position += length + 3;
		}

		return packets;
	}
}
