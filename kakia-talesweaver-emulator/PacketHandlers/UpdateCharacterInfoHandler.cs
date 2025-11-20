using kakia_talesweaver_emulator.DB;
using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;

namespace kakia_talesweaver_emulator.PacketHandlers;

[PacketHandlerAttr(PacketType.UpdateCharacterInfo)]
public class UpdateCharacterInfoHandler : PacketHandler
{
	public override void HandlePacket(IPlayerClient client, RawPacket p)
	{
		// Not entirely sure if this packet is this.
		// But it's sent with 0F when character is leaving the map/logging out.

		var session = client.GetSessionInfo();
		if (session == null) 
		{
			return;
		}
		if (session.Character == null)
		{
			return;
		}

		JsonDB.SaveCharacter(session.AccountId, session.Character);		 
	}
}
