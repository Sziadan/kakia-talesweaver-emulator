using kakia_talesweaver_emulator.DB;
using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using kakia_talesweaver_packets.Packets;
using kakia_talesweaver_utils.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.Login)]
public class LoginHandler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		var login = LoginPacket.FromBytes(p.Data);
		if (login is null)
			return;

		(client as PlayerClient)!.AccountId = login.Username;

		var charlist = JsonDB.GetCharacterList(login.Username);

		CharacterListPacket clp = new()
		{
			AccountId = [new AccountIdInfo()
			{
				ServerId = 26,
				Name = login.Username,
				CharacterCount = (byte)charlist.Count
			}]
		};
		
		client.Send(clp.ToBytes(), CancellationToken.None).Wait();
	}
}