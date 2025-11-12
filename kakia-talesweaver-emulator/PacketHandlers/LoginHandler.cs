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

		(client as PlayerClient)!.AccountName = login.Username;

		CharacterListPacket clp = new();
		clp.Characters.Add(new CharacterInfo()
		{
			ServerId = 26,
			Name = login.Username
		});

		client.Send(clp.ToBytes(), CancellationToken.None).Wait();
	}
}