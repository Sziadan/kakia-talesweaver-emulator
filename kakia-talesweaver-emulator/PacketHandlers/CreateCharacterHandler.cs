using kakia_talesweaver_emulator.DB;
using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_packets.Models;
using kakia_talesweaver_packets.Packets;
using kakia_talesweaver_utils;
using kakia_talesweaver_utils.Extensions;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.CreateCharacter)]
public class CreateCharacterHandler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		using PacketReader reader = new(p.Data);
		reader.Skip(1); // Packet ID

		Character character = new()
		{
			LastLoginTime = 0,
			CreationTime = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
			ModelId = reader.ReadUInt32BE(),
			Name = reader.ReadPrefixedString()
		};

		var session = client.GetSessionInfo();
		if (session == null)
		{
			// Invalid session, ignore packet
			return;
		}

		var characters = JsonDB.GetCharacterList(session.AccountId);
		characters.Add(character);
		var charSelectList = new CharacterSelectList() { Characters = characters };

		client.Send(@"7C 01 00 33 FB 13 4F 98 D6 2D 48 63 40 22 A2 58 
					0F 66 1A 71 8E 36 1B 0C 90 6B 10 51 A4 6D 9B 66 
					D0 2D 51 27 1E 5D BA 65 12 5B F7 65 2D 7B C9 75 
					6E 03 84 1D 00 00 00 00".ToByteArray(), CancellationToken.None).Wait();

		client.Send("02 00 00 00 00".ToByteArray(), CancellationToken.None).Wait();

		using PacketWriter writer = new();
		writer.Write((byte)0x6B);
		writer.Write((short)0);
		writer.Write((byte)1);
		writer.Write((byte)0);
		writer.Write((byte)characters.Count);
		client.Send(writer.ToArray(), CancellationToken.None).Wait();

		client.Send(charSelectList.ToBytes(), CancellationToken.None).Wait();

		character.LastLoginTime = 946566000;
		character.Id = (uint)characters.IndexOf(character) + 2000000;
		JsonDB.SaveCharacter(session.AccountId, character);

		// TODO: Validate character creation (name uniqueness, model validity, etc.)



	}
}
