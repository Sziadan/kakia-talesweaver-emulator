using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_network;
using kakia_talesweaver_packets;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace kakia_talesweaver_emulator.PacketHandlers;

public class PacketHandlerAttr : Attribute
{
	public PacketType packetId;
	public PacketHandlerAttr(PacketType id)
	{
		this.packetId = id;
	}
}

public abstract class PacketHandler
{
	public abstract void HandlePacket(IPlayerClient client, RawPacket p);
}

public static class PacketHandlers
{
	private static readonly Dictionary<PacketType, PacketHandler> Handlers = new Dictionary<PacketType, PacketHandler>();

	public static void LoadPacketHandlers(string namespaceName)
	{
		var classes = from t in Assembly.GetCallingAssembly().GetTypes()
					  where
						  t.IsClass && t.Namespace == namespaceName &&
						  t.IsSubclassOf(typeof(PacketHandler))
					  select t;

		foreach (var t in classes.ToList())
		{
			var attrs = (Attribute[])t.GetCustomAttributes(typeof(PacketHandlerAttr), false);

			if (attrs.Length > 0)
			{
				var attr = (PacketHandlerAttr)attrs[0];
				if (!Handlers.ContainsKey(attr.packetId))
					Handlers.Add(attr.packetId, (PacketHandler)Activator.CreateInstance(t));
			}
		}
	}

	public static PacketHandler GetHandlerFor(PacketType id)
	{
		PacketHandler handler = null;

		if (Handlers.ContainsKey(id))
			Handlers.TryGetValue(id, out handler);

		return handler;
	}

	public static PacketHandler[] GetLoadedHandlers()
	{
		return Handlers.Values.ToArray();
	}
}