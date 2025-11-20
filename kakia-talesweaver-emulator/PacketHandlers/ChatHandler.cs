using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_packets.Models;
using kakia_talesweaver_packets.Packets;
using kakia_talesweaver_utils;
using kakia_talesweaver_utils.Extensions;
using System.Net.Http.Headers;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.Chat)]
public class ChatHandler : PacketHandler
{
	
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var packet = ChatPacket.FromBytes(p.Data);
		packet.CharacterId = client.GetCharacter().Id;
		client.Broadcast(packet.ToBytes(), includeSelf: true);

		string[] messageCmd = packet.Message.Split(" ");
		switch (messageCmd[0])
		{
			case "@warp":
				if (TalesServer.Maps.TryGetValue(messageCmd[1], out MapInfo? map))
				{
					client.LoadMap(map, true);
				}
				break;

			case "@listmaps":
				var cAnswer = new ChatPacket
				{
					CharacterId = 0,
					Message = string.Join(", ", TalesServer.Maps.Keys)
				};
				client.Send(cAnswer.ToBytes(), CancellationToken.None);
				break;

			case "@startduel":
				client.Send(new SendCharEffectPacket() { 
					ObjectId = packet.CharacterId,
					Effect = CharEffect.PvpCountDownTimer}
				.ToBytes(), CancellationToken.None).Wait();
				break;

			case "@levelup":
				client.Send(new SendCharEffectPacket()
				{
					ObjectId = packet.CharacterId,
					Effect = CharEffect.LevelUp
				}
				.ToBytes(), CancellationToken.None).Wait();
				break;

			case "@maxlevel":
				client.Send(new SendCharEffectPacket()
				{
					ObjectId = packet.CharacterId,
					Effect = CharEffect.MaxLevel
				}
				.ToBytes(), CancellationToken.None).Wait();
				break;

			case "@test":
				{
					using PacketWriter pw = new();
					pw.Write((byte)0x1A);
					pw.Write(packet.CharacterId);
					pw.Write((byte)0x08);
					client.Send(pw.ToArray(), CancellationToken.None).Wait();
					break;
				}

			case "@refresh":
				var character = client.GetCharacter();
				client.Send(character.SpawnCharacterPacket!.ToBytes(), CancellationToken.None).Wait();
				break;

			default:
				break;
		}
	}
}