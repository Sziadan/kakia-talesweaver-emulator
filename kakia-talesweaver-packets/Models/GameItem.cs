using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Models;

public class GameItem
{
	public int ItemID { get; set; }
	public short Amount { get; set; }
	public short Durability { get; set; }

	// Equipment specific data
	public byte Refine { get; set; } // +1, +2, etc.
	public byte DataFlags { get; set; } // Rarity, Tradeable, etc.
	public List<ItemStat> Stats { get; set; } = new();

	/// <summary>
	/// Parses the variable-length GameItem structure.
	/// </summary>
	public static GameItem Parse(PacketReader reader)
	{
		var item = new GameItem();

		// 1. Read Item ID
		item.ItemID = reader.ReadInt32BE();

		// If ItemID is 0 or -1 (depending on version), the slot is empty.
		// Usually 0x07 packets won't send an item with ID 0.
		if (item.ItemID <= 0) return item;

		// 2. Read Basic Info
		item.Amount = reader.ReadInt16BE();
		item.Durability = reader.ReadInt16BE();

		// 3. Determine if the item has "Extended Data" (Stats)
		// This logic is internal to the game client. 
		// Generally, IDs > 2000 (or specific ranges) are equipment.
		// Stackable items (potions, etc) usually DO NOT have the following bytes.
		if (IsEquipment(item.ItemID))
		{
			item.Refine = reader.ReadByte();
			item.DataFlags = reader.ReadByte();

			// 4. Read Stats
			// The packet sends a byte count, followed by [Type:Short, Value:Int] pairs.
			byte statCount = reader.ReadByte();

			for (int i = 0; i < statCount; i++)
			{
				short type = reader.ReadInt16BE();
				int value = reader.ReadInt32BE();
				item.Stats.Add(new ItemStat(type, value));
			}
		}

		return item;
	}

	public byte[] ToBytes()
	{
		using PacketWriter pw = new();
		pw.Write(ItemID);
		pw.Write(Amount);
		pw.Write(Durability);
		if (IsEquipment(ItemID))
		{
			pw.Write(Refine);
			pw.Write(DataFlags);
			pw.Write((byte)Stats.Count);
			foreach (var stat in Stats)
			{
				pw.Write(stat.Type);
				pw.Write(stat.Value);
			}
		}
		return pw.ToArray();
	}

	private static bool IsEquipment(int id)
	{
		// Example logic:
		// 0-2000: Consumption/Etc (No stats)
		// 2000+: Equipment (Has stats)
		// Note: Some versions have "Quest Items" that act differently.
		return id > 2000;
	}
}

public struct ItemStat
{
	public short Type;
	public int Value;
	public ItemStat(short t, int v) { Type = t; Value = v; }
}
