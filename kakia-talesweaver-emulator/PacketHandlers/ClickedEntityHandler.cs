using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_utils;
using kakia_talesweaver_utils.Extensions;
using System.Buffers.Binary;
using System.Text;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.ClickedEntity)]
public class ClickedEntityHandler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		uint entityId = BinaryPrimitives.ReadUInt32BigEndian(p.Data.AsSpan().Slice(1, 4));
		using PacketWriter writer = new();
		writer.Write((byte)0x70);
		writer.Write((byte)0);
		writer.Write(entityId);
		writer.Write((uint)0);
		writer.Write((byte)0);

		client.Send(writer.ToArray(), CancellationToken.None).Wait();


		client.Send(@"83 02 00 00 4C 4E 31 01 00 00 00 00 00 00 00 00
					  00 00 00 00 01 00 00 00 00 01 00 00 00 00 01 00
					  00 00 00 01 00 00 00 00".ToByteArray(), CancellationToken.None).Wait();

		client.Send(@"83 02 00 00 4C 4E 00 01 00 00 00 00 00 00 00 00
					  00 00 00 00 01 00 00 00 00 01 00 00 00 00 01 00
					  00 00 00 01 00 00 00 00".ToByteArray(), CancellationToken.None).Wait();

		client.Send(@"83 02 00 00 3D 29 94 01 00 00 00 00 00 00 00 00
					  00 00 00 00 01 00 00 00 00 01 00 00 00 00 01 00
					  00 00 00 01 00 00 00 00".ToByteArray(), CancellationToken.None).Wait();


		client.Send(@"14 01 00 00 00".ToByteArray(), CancellationToken.None).Wait();
		client.Send(@"49 0B 00 00 00 00 0B B8 00 00 00 00 00 00 00 00".ToByteArray(), CancellationToken.None).Wait();

		client.Send(writer.ToArray(), CancellationToken.None).Wait();

		client.Send("0B 01 03 C0 68 24".ToByteArray(), CancellationToken.None).Wait();
		client.Send("11 03 C0 68 24 07".ToByteArray(), CancellationToken.None).Wait();


		if (entityId == 1946812928)
		{

			using PacketWriter pwMsg = new();
			pwMsg.Write((byte)0x44);
			pwMsg.Write((byte)0x04); // 04 => Dialog ?
			pwMsg.Write((byte)0x02); // 02 => With options


			pwMsg.Write((byte)0x03); // Unk
			pwMsg.Write((byte)0xC0);
			pwMsg.Write((byte)0x68);
			pwMsg.Write((byte)0x24);

			pwMsg.Write((byte)0x03);
			pwMsg.Write((byte)0xC0);
			pwMsg.Write((byte)0x6C);
			pwMsg.Write((byte)0xA0);

			pwMsg.Write((byte)0x00); //-
			pwMsg.Write((byte)0x21); //- NPC Model id (in dialog image)
			pwMsg.Write((byte)0x97); //- 
			pwMsg.Write((byte)0xC1); //-


			pwMsg.Write((byte)0x00);

			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			string message = "Isn't Kakia TalesWeaver Private Server\njust the greatest thing to exist since\nsliced bread?!?!";
			byte[] messageBytes = Encoding.GetEncoding("shift-jis").GetBytes(message);

			pwMsg.Write((byte)messageBytes.Length);
			pwMsg.Write(messageBytes);

			pwMsg.Write((byte)0x04);
			pwMsg.Write((byte)0x00);

			string resp1 = "Absolutely! Even told my grandma about it!";
			byte[] resp1Bytes = Encoding.GetEncoding("shift-jis").GetBytes(resp1);

			pwMsg.Write((byte)resp1Bytes.Length);
			pwMsg.Write(resp1Bytes);
			pwMsg.Write((byte)0x00);


			string resp2 = "You know what, it may even surpass the wheel!";
			byte[] resp2Bytes = Encoding.GetEncoding("shift-jis").GetBytes(resp2);

			pwMsg.Write((byte)resp2Bytes.Length);
			pwMsg.Write(resp2Bytes);
			pwMsg.Write((byte)0x00);

			string resp4 = "Probably, most likely, yeah.";
			byte[] resp4Bytes = Encoding.GetEncoding("shift-jis").GetBytes(resp4);

			pwMsg.Write((byte)resp4Bytes.Length);
			pwMsg.Write(resp4Bytes);
			pwMsg.Write((byte)0x00);

			string resp3 = "...Mom told me not to talk to strangers.";
			byte[] resp3Bytes = Encoding.GetEncoding("shift-jis").GetBytes(resp3);

			pwMsg.Write((byte)resp3Bytes.Length);
			pwMsg.Write(resp3Bytes);
			pwMsg.Write((byte)0x00);



			client.Send(pwMsg.ToArray(), CancellationToken.None).Wait();
		}
		else if (entityId == 1779040768)
		{

			client.Send("44 05 00 00 01 00".ToByteArray(), CancellationToken.None).Wait();


			using PacketWriter pwMsg = new();
			pwMsg.Write((byte)0x44);
			pwMsg.Write((byte)05);
			pwMsg.Write((byte)02);
			pwMsg.Write((byte)0);
			pwMsg.Write((byte)0x21);
			pwMsg.Write((byte)0x97);
			pwMsg.Write((byte)0xC2);
			pwMsg.Write((int)0);

			string message = "I'm really curious about J-Ren's hair.</n>Look at it, it's not groomed and it's completely ruffled, right?</n>You should make sure to take good care of your hair.";
			byte[] respBytes = Encoding.ASCII.GetBytes(message);

			pwMsg.Write((byte)respBytes.Length);
			pwMsg.Write(respBytes);

			byte[] pck = pwMsg.ToArray();
			string pckStr = pck.ToFormatedHexString();
			client.Send(pck, CancellationToken.None).Wait();

			/*
			client.Send(@"44 05 02 00 21 97 C2 00 00 00 00 9A 83 4A 83 8C
83 93 82 CC 94 AF 82 CC 96 D1 82 AA 82 A0 82 DC
82 E8 82 C9 82 E0 8B 43 82 C9 82 C8 82 C1 82 C4
81 42 3C 2F 6E 3E 82 A0 82 EA 82 F0 8C A9 82 C4
81 41 8E E8 93 FC 82 EA 82 F0 82 B5 82 C4 82 A2
82 C8 82 A2 82 B5 81 41 8A AE 91 53 82 C9 8C A2
96 D1 82 B6 82 E1 82 C8 82 A2 82 CC 81 48 82 A0
82 C8 82 BD 82 E0 94 AF 82 CC 96 D1 82 CC 8E E8
93 FC 82 EA 82 CD 82 B5 82 C1 82 A9 82 E8 82 C6
82 B5 82 C4 82 A8 82 A2 82 BD 95 FB 82 AA 97 C7
82 A2 82 E6 81 42".ToByteArray(), CancellationToken.None).Wait();
			*/

			client.Send("44 05 05 03 C0 68 24 03 C0 71 9B".ToByteArray(), CancellationToken.None).Wait();

			client.Send("44 05 01".ToByteArray(), CancellationToken.None).Wait();
		}
		else if (entityId == 185205248) // Outfitter
		{
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
}
