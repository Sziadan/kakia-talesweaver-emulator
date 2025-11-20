using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_utils;
using System.Text;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.NpcPickedDialogAnswer)]
public class NpcPickedDialogAnswerHandler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		using var pr = new PacketReader(p.Data);
		pr.Skip(1); // Packet ID
		byte flag1 = pr.ReadByte();
		byte flag2 = pr.ReadByte();

		if (flag2 == 5) // closed window
			return;

		ulong dialogId = pr.ReadUInt64BE();

		switch(dialogId)
		{
			case 1:
				pr.Skip(2);
				BeautyHandler(pr.ReadByte(), client);
				break;


			default:
				break;
		}
	}

	public void BeautyHandler(uint option, IPlayerClient client)
	{
		var character = client.GetCharacter();
		character.SpawnCharacterPacket.Position.Position = character.Position;
		character.SpawnCharacterPacket.Position.Direction = character.Direction;
		switch (option)
		{
			case 0:
				character.SpawnCharacterPacket!.Outfit++;
				break;

			case 1:
				character.SpawnCharacterPacket!.Outfit--;
				break;

			case 2:
				character.SpawnCharacterPacket!.Outfit = 0;
				client.Send(character.SpawnCharacterPacket!.ToBytes(), CancellationToken.None).Wait();
				return;

			case 3:
			default:
				return;
		}
		client.Send(character.SpawnCharacterPacket!.ToBytes(), CancellationToken.None).Wait();

		using PacketWriter pwMsg = new();
		pwMsg.Write((byte)0x44);
		pwMsg.Write((byte)0x04); // 04 => Dialog ?
		pwMsg.Write((byte)0x02); // 02 => With options

		ulong dialogAnswerId = 1;
		uint modelId = 2201600;

		pwMsg.Write(dialogAnswerId);
		pwMsg.Write(modelId);
		pwMsg.Write((byte)0x00);

		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

		string message = "Dress to impress!\nWant to try out a new outfit?\nPerhaps add some flair?";
		byte[] messageBytes = Encoding.GetEncoding("shift-jis").GetBytes(message);

		pwMsg.Write((byte)messageBytes.Length);
		pwMsg.Write(messageBytes);

		pwMsg.Write((byte)0x04);
		pwMsg.Write((byte)0x00);

		string resp1 = "Let try the next one.";
		byte[] resp1Bytes = Encoding.GetEncoding("shift-jis").GetBytes(resp1);

		pwMsg.Write((byte)resp1Bytes.Length);
		pwMsg.Write(resp1Bytes);
		pwMsg.Write((byte)0x00);


		string resp2 = "Let me try the previous one.";
		byte[] resp2Bytes = Encoding.GetEncoding("shift-jis").GetBytes(resp2);

		pwMsg.Write((byte)resp2Bytes.Length);
		pwMsg.Write(resp2Bytes);
		pwMsg.Write((byte)0x00);

		string resp4 = "Turn me back to normal.";
		byte[] resp4Bytes = Encoding.GetEncoding("shift-jis").GetBytes(resp4);

		pwMsg.Write((byte)resp4Bytes.Length);
		pwMsg.Write(resp4Bytes);
		pwMsg.Write((byte)0x00);

		string resp3 = "Nevermind.";
		byte[] resp3Bytes = Encoding.GetEncoding("shift-jis").GetBytes(resp3);

		pwMsg.Write((byte)resp3Bytes.Length);
		pwMsg.Write(resp3Bytes);
		pwMsg.Write((byte)0x00);

		client.Send(pwMsg.ToArray(), CancellationToken.None).Wait();
	}
}
