using System;
using System.Collections.Generic;
using System.Text;
using kakia_talesweaver_packets.Models;
using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class SpawnCharacterPacket
{
	// Packet identifiers
	public byte PacketId { get; set; } = 0x33;
	public byte SubOpcode { get; set; } = 0x00; // spawn user

	// Main identifier used in parser
	public uint UserId { get; set; }
	public bool IsThisPlayer { get; set; }
	public byte GM { get; set; } = 0;
	public byte UnkByte1 { get; set; } = 0;
	public short UnkShort1 { get; set; } = 0;
	public ObjectPos Position { get; set; } = new();
	public byte Unk { get; set; }

	// Basic stats block
	public BasicStatsBlock BasicStats { get; set; } = new();

	// Name (1-byte length prefixed in the protocol)
	public string UserName { get; set; } = string.Empty;

	// Guild / economy
	public uint GuildId { get; set; }
	public ulong CurrentHealth { get; set; }
	public ulong MaxHealth { get; set; }

	// Guild/team (conditional)
	public GuildTeamInfo GuildTeam { get; set; } = new();

	// Misc fields read after guild/team
	public byte UnknownByte3 { get; set; }
	public ushort Unk2 { get; set; }
	public ushort Outfit { get; set; }
	public uint UnknownInt4 { get; set; }
	public uint ModelId { get; set; }

	// Equipment list (22 slots represented compactly by a bitmask in the stream)
	public List<ItemSlotData> Equipment { get; set; } = new();

	// Two bytes (LOBYTE / HIBYTE of some earlier value in parser)
	public byte UnknownByte4 { get; set; }
	public byte UnknownByte5 { get; set; }

	// Appearance / colors
	public AppearanceBlock Appearance { get; set; } = new();

	public uint UnknownInt6 { get; set; }

	// Conditional guild name (present when a preceding flag == 1)
	public string? ConditionalGuildName { get; set; }

	public uint UnknownInt7 { get; set; }
	public uint EquippedTitle { get; set; }

	// User state (complex)
	public UserStateBlock UserState { get; set; } = new();

	// Final status values
	public FinalStatusBlock FinalStatus { get; set; } = new();

	// --- Helper nested types to reflect Python parser ---

	public class MovementInfo
	{
		public uint UnknownInt1 { get; set; }
		public ushort XPos { get; set; }
		public ushort YPos { get; set; }
		public byte[] MovePathData { get; set; } = new byte[2]; // 2 bytes
	}

	public class BasicStatsBlock
	{
		public byte UnknownFlag1 { get; set; }
		public byte UnknownFlag2 { get; set; }
		public ushort UnknownStat1 { get; set; }
		public ushort UnknownStat2 { get; set; }
		public ushort UnknownStat3 { get; set; }
		public ushort UnknownStat4 { get; set; }
	}

	public class GuildTeamInfo
	{
		public byte HasGuildInfo { get; set; }
		public string GuildName { get; set; } = string.Empty;
		public uint GuildId { get; set; }
		public string TeamName { get; set; } = string.Empty;
		public ushort UnknownShort1 { get; set; }
		public ushort UnknownShort2 { get; set; }
	}

	public class ItemMagicProperty
	{
		public byte PropertyId { get; set; }
		public byte PropertyType { get; set; }
		public uint PropertyValue1 { get; set; }
		// Depending on PropertyType other union-like fields follow
		public uint? PropertyValue2 { get; set; }
		public ushort[]? StatValues { get; set; }
		public uint[]? StatParams { get; set; }
		public int? UnknownSkillInt { get; set; }
		public List<SkillEntryStruct>? Skills { get; set; }
		public byte? PropertyValue6 { get; set; }

		public class SkillEntryStruct
		{
			public uint SkillId { get; set; }
			public bool IsActive { get; set; }
			public uint[] Params { get; set; } = new uint[5];
		}
	}

	public class ItemSlotData
	{
		public int Slot { get; set; } = -1; // slot index 0..21
		public uint ItemId { get; set; }
		public uint VisualId { get; set; }

		public StatBlock[] StatBlocks { get; set; } = new StatBlock[2];

		public ushort DurabilityOrCount { get; set; }
		public List<ItemMagicProperty> MagicProperties { get; set; } = new();

		public ItemSlotData()
		{
			StatBlocks[0] = new StatBlock();
			StatBlocks[1] = new StatBlock();
		}

		public class StatBlock
		{
			public byte HasStats { get; set; }
			public byte StatType { get; set; }
			public uint[] Stats { get; set; } = new uint[6];
		}
	}

	public class AppearanceBlock
	{
		public byte UnknownAppearanceFlag { get; set; }
		public List<AppearanceSlot> Slots { get; set; } = new();
		public List<AppearanceProp> Properties { get; set; } = new();

		public class AppearanceSlot
		{
			public int SlotIndex { get; set; }
			public byte Type { get; set; }
			public uint? VisualId1 { get; set; }
			public uint? VisualId2 { get; set; }
			public byte? ColorOrIndex { get; set; }
		}

		public class AppearanceProp
		{
			public int PropIndex { get; set; }
			public uint Id { get; set; }
			public byte Type { get; set; }
			public uint? ValueInt { get; set; }
			public byte? ValueByte { get; set; }
		}
	}

	public class UserStateBlock
	{
		public byte UnknownFlag1 { get; set; }
		public uint[] StateData { get; set; } = new uint[8];
		public bool UnknownBool { get; set; }
		public byte UnknownFlag2 { get; set; }
		public byte UnknownDiscardedByte { get; set; }
		public byte UnknownFlag3 { get; set; }
	}

	// Probably pet info
	public class FinalStatusBlock
	{
		public byte UnknownByte6 { get; set; }
		public bool UnknownBool2 { get; set; }
		public byte UnknownByte7 { get; set; }
		public uint CurrentHP { get; set; }
		public uint MaxHP { get; set; }
		public uint CurrentMP { get; set; }
		public ulong CurrentSP { get; set; }
		public byte UnknownByte8 { get; set; }
	}

	// Full serializer matching Python build_packet_0x33
	public byte[] ToBytes(bool SetAsOther = false)
	{
		using PacketWriter pw = new();

		// Packet ID + sub-opcode + user id
		pw.Write(PacketId);
		pw.Write(SubOpcode);
		pw.Write(UserId);

		//pw.Write((byte)(IsThisPlayer ? 1 : 0));

		if (SetAsOther)
			pw.Write((byte)0);
		else
			pw.Write((byte)1);

		// --- spawn user payload ---
		// Movement
		pw.Write(GM);
		pw.Write(UnkByte1);
		pw.Write(UnkShort1);
		pw.Write(Position.ToBytes());
		pw.Write(Unk);

		// Basic stats
		pw.Write(BasicStats.UnknownFlag1);
		pw.Write(BasicStats.UnknownFlag2);
		pw.Write(BasicStats.UnknownStat1);
		pw.Write(BasicStats.UnknownStat2);
		pw.Write(BasicStats.UnknownStat3);
		pw.Write(BasicStats.UnknownStat4);

		// UserName (1-byte length-prefixed, latin-1)
		_pack_string(pw, UserName);

		pw.Write(GuildId);
		pw.Write(CurrentHealth);
		pw.Write(MaxHealth);

		_pack_guild_team(pw, GuildTeam);

		pw.Write(UnknownByte3);
		pw.Write(Unk2);
		pw.Write(Outfit);		
		pw.Write(UnknownInt4);
		pw.Write(ModelId);

		_pack_equipment(pw, Equipment);

		pw.Write(UnknownByte4);
		pw.Write(UnknownByte5);

		_pack_appearance(pw, Appearance);

		pw.Write(UnknownInt6);

		if (!string.IsNullOrEmpty(ConditionalGuildName))
		{
			pw.Write((byte)1);
			_pack_string(pw, ConditionalGuildName!);
		}
		else
		{
			pw.Write((byte)0);
		}

		pw.Write(UnknownInt7);
		pw.Write(EquippedTitle);

		_pack_user_state(pw, UserState);

		// Final status
		pw.Write(FinalStatus.UnknownByte6);
		pw.Write((byte)(FinalStatus.UnknownBool2 ? 1 : 0));
		pw.Write(FinalStatus.UnknownByte7);
		pw.Write(FinalStatus.CurrentHP);
		pw.Write(FinalStatus.MaxHP);
		pw.Write(FinalStatus.CurrentMP);
		pw.Write(FinalStatus.CurrentSP);
		pw.Write(FinalStatus.UnknownByte8);

		return pw.ToArray();
	}

	private static void _pack_string(PacketWriter pw, string text)
	{
		var bytes = Encoding.Latin1.GetBytes(text);
		if (bytes.Length > 255) throw new ArgumentException("String too long for 1-byte length prefix");
		pw.Write((byte)bytes.Length);
		pw.Write(bytes);
	}

	private static void _pack_guild_team(PacketWriter pw, GuildTeamInfo info)
	{
		pw.Write(info.HasGuildInfo);
		if (info.HasGuildInfo != 0)
		{
			_pack_string(pw, info.GuildName);
			pw.Write(info.GuildId);
			_pack_string(pw, info.TeamName);
			pw.Write(info.UnknownShort1);
			pw.Write(info.UnknownShort2);
		}
	}

	private static void _pack_item_magic_properties(PacketWriter pw, List<ItemMagicProperty> properties)
	{
		pw.Write((byte)properties.Count);
		foreach (var prop in properties)
		{
			pw.Write(prop.PropertyId);
			pw.Write(prop.PropertyType);
			pw.Write(prop.PropertyValue1);

			switch (prop.PropertyType)
			{
				case 1:
					pw.Write(prop.PropertyValue2 ?? 0);
					break;
				case 2:
					{
						ushort[] vals = prop.StatValues ?? new ushort[12];
						for (int i = 0; i < 12; i++) pw.Write(vals[i]);
					}
					break;
				case 3:
					pw.Write(prop.UnknownSkillInt ?? 0);
					pw.Write((byte)(prop.Skills?.Count ?? 0));
					if (prop.Skills != null)
					{
						foreach (var s in prop.Skills)
						{
							pw.Write(s.SkillId);
							pw.Write((byte)(s.IsActive ? 1 : 0));
							foreach (var p in s.Params) pw.Write(p);
						}
					}
					break;
				case 4:
				case 5:
				case 6:
					pw.Write(prop.PropertyValue2 ?? 0);
					break;
				case 7:
				case 9:
					{
						ushort[] statVals = prop.StatValues ?? new ushort[12];
						for (int i = 0; i < 12; i++) pw.Write(statVals[i]);
						uint[] statParams = prop.StatParams ?? new uint[12];
						for (int i = 0; i < 12; i++) pw.Write(statParams[i]);
					}
					break;
				case 8:
					pw.Write(prop.PropertyValue6 ?? 0);
					break;
			}
		}
	}

	private static void _pack_item_slot_data(PacketWriter pw, ItemSlotData item)
	{
		pw.Write(item.ItemId);
		pw.Write(item.VisualId);

		foreach (var block in item.StatBlocks)
		{
			pw.Write(block.HasStats);
			if (block.HasStats != 0)
			{
				pw.Write(block.StatType);
				foreach (var s in block.Stats) pw.Write(s);
			}
		}

		pw.Write(item.DurabilityOrCount);
		_pack_item_magic_properties(pw, item.MagicProperties);
	}

	private static void _pack_equipment(PacketWriter pw, List<ItemSlotData> equipment)
	{
		uint equipment_bitmask = 0;
		ItemSlotData?[] full = new ItemSlotData?[22];
		foreach (var it in equipment)
		{
			if (it.Slot >= 0 && it.Slot < 22)
			{
				equipment_bitmask |= (1u << it.Slot);
				full[it.Slot] = it;
			}
		}

		pw.Write((int)equipment_bitmask);

		for (int i = 0; i < 22; i++)
		{
			var it = full[i];
			if (it != null) _pack_item_slot_data(pw, it);
		}
	}

	private static void _pack_appearance(PacketWriter pw, AppearanceBlock appearance)
	{
		pw.Write(appearance.UnknownAppearanceFlag);

		for (int i = 0; i < 9; i++)
		{
			var slot = appearance.Slots.Find(s => s.SlotIndex == i);
			byte type = slot?.Type ?? 0;
			pw.Write(type);
			if (type == 1)
			{
				pw.Write(slot?.VisualId1 ?? 0);
				pw.Write(slot?.VisualId2 ?? 0);
			}
			else
			{
				pw.Write(slot?.ColorOrIndex ?? 0);
			}
		}

		for (int i = 0; i < 4; i++)
		{
			var prop = appearance.Properties.Find(p => p.PropIndex == i);
			pw.Write(prop?.Id ?? 0);
			pw.Write(prop?.Type ?? 0);
			if (prop != null)
			{
				if (prop.Type == 1) pw.Write(prop.ValueInt ?? 0);
				else pw.Write(prop.ValueByte ?? 0);
			}
		}
	}

	private static void _pack_user_state(PacketWriter pw, UserStateBlock state)
	{
		pw.Write(state.UnknownFlag1);
		for (int i = 0; i < 8; i++) pw.Write(state.StateData[i]);
		pw.Write((byte)(state.UnknownBool ? 1 : 0));
		pw.Write(state.UnknownFlag2);
		pw.Write(state.UnknownDiscardedByte);
		pw.Write(state.UnknownFlag3);
	}

	// --- PARSING / FromBytes ---
	public static SpawnCharacterPacket FromBytes(byte[] data)
	{
		var packet = new SpawnCharacterPacket();
		var reader = new PacketReader(data);
		try
		{
			// Handle both cases: data may start with 0x33 or with sub-opcode
			byte first = reader.ReadByte();
			byte subop;
			if (first == 0x33)
			{
				// skip packet id
				subop = reader.ReadByte();
			}
			else
			{
				subop = first;
			}
			packet.SubOpcode = subop;
			packet.UserId = reader.ReadUInt32BE();

			if (subop != 0)
			{
				// Not a spawn-user payload; return header info only
				return packet;
			}

			packet.IsThisPlayer = reader.ReadByte() != 0;

			// Movement
			packet.GM = reader.ReadByte();
			packet.UnkByte1 = reader.ReadByte();
			packet.UnkShort1 = reader.ReadInt16BE();
			packet.Position.Position.X = reader.ReadUInt16BE();
			packet.Position.Position.Y = reader.ReadUInt16BE();
			packet.Position.Direction = reader.ReadByte();
			packet.Unk = reader.ReadByte();

			// Basic stats
			packet.BasicStats.UnknownFlag1 = reader.ReadByte();
			packet.BasicStats.UnknownFlag2 = reader.ReadByte();
			packet.BasicStats.UnknownStat1 = reader.ReadUInt16BE();
			packet.BasicStats.UnknownStat2 = reader.ReadUInt16BE();
			packet.BasicStats.UnknownStat3 = reader.ReadUInt16BE();
			packet.BasicStats.UnknownStat4 = reader.ReadUInt16BE();

			// UserName
			packet.UserName = ReadPrefixedStringLatin1(reader);

			// Guild/economy
			packet.GuildId = reader.ReadUInt32BE();
			packet.CurrentHealth = ReadUInt64BE(reader);
			packet.MaxHealth = ReadUInt64BE(reader);

			// Guild/team
			packet.GuildTeam = _parse_guild_team(reader);

			packet.UnknownByte3 = reader.ReadByte();
			packet.Unk2 = reader.ReadUInt16BE();
			packet.Outfit = reader.ReadUInt16BE();			
			packet.UnknownInt4 = reader.ReadUInt32BE();
			packet.ModelId = reader.ReadUInt32BE();

			// Equipment
			packet.Equipment = _parse_equipment(reader);

			packet.UnknownByte4 = reader.ReadByte();
			packet.UnknownByte5 = reader.ReadByte();

			packet.Appearance = _parse_appearance(reader);

			packet.UnknownInt6 = reader.ReadUInt32BE();

			byte hasCondGuild = reader.ReadByte();
			if (hasCondGuild != 0)
			{
				packet.ConditionalGuildName = ReadPrefixedStringLatin1(reader);
			}
			else
			{
				packet.ConditionalGuildName = null;
			}

			packet.UnknownInt7 = reader.ReadUInt32BE();
			packet.EquippedTitle = reader.ReadUInt32BE();

			packet.UserState = _parse_user_state(reader);

			// Final status
			packet.FinalStatus.UnknownByte6 = reader.ReadByte();
			packet.FinalStatus.UnknownBool2 = reader.ReadByte() != 0;
			packet.FinalStatus.UnknownByte7 = reader.ReadByte();
			packet.FinalStatus.CurrentHP = reader.ReadUInt32BE();
			packet.FinalStatus.MaxHP = reader.ReadUInt32BE();
			packet.FinalStatus.CurrentMP = reader.ReadUInt32BE();
			packet.FinalStatus.CurrentSP = ReadUInt64BE(reader);
			packet.FinalStatus.UnknownByte8 = reader.ReadByte();
		}
		catch (EndOfStreamException)
		{
			// Input shorter than expected; return what we have
			return packet;
		}
		return packet;
	}

	private static string ReadPrefixedStringLatin1(PacketReader reader)
	{
		int len = reader.ReadByte();
		var bytes = reader.ReadBytes(len);
		return Encoding.Latin1.GetString(bytes);
	}

	private static ulong ReadUInt64BE(PacketReader reader)
	{
		var b = reader.ReadBytes(8);
		if (b.Length < 8) throw new EndOfStreamException();
		Array.Reverse(b);
		return BitConverter.ToUInt64(b, 0);
	}

	private static GuildTeamInfo _parse_guild_team(PacketReader reader)
	{
		var info = new GuildTeamInfo();
		info.HasGuildInfo = reader.ReadByte();
		if (info.HasGuildInfo != 0)
		{
			info.GuildName = ReadPrefixedStringLatin1(reader);
			info.GuildId = reader.ReadUInt32BE();
			info.TeamName = ReadPrefixedStringLatin1(reader);
			info.UnknownShort1 = reader.ReadUInt16BE();
			info.UnknownShort2 = reader.ReadUInt16BE();
		}
		return info;
	}

	private static List<ItemMagicProperty> _parse_item_magic_properties(PacketReader reader)
	{
		var list = new List<ItemMagicProperty>();
		int propertyCount = reader.ReadByte();
		for (int i = 0; i < propertyCount; i++)
		{
			var prop = new ItemMagicProperty();
			prop.PropertyId = reader.ReadByte();
			prop.PropertyType = reader.ReadByte();
			prop.PropertyValue1 = reader.ReadUInt32BE();

			switch (prop.PropertyType)
			{
				case 1:
					prop.PropertyValue2 = reader.ReadUInt32BE();
					break;
				case 2:
					{
						var vals = new ushort[12];
						for (int j = 0; j < 12; j++) vals[j] = reader.ReadUInt16BE();
						prop.StatValues = vals;
					}
					break;
				case 3:
					prop.UnknownSkillInt = (int)reader.ReadUInt32BE();
					int skillCount = reader.ReadByte();
					prop.Skills = new List<ItemMagicProperty.SkillEntryStruct>();
					for (int s = 0; s < skillCount; s++)
					{
						var sk = new ItemMagicProperty.SkillEntryStruct();
						sk.SkillId = reader.ReadUInt32BE();
						sk.IsActive = reader.ReadByte() != 0;
						for (int p = 0; p < 5; p++) sk.Params[p] = reader.ReadUInt32BE();
						prop.Skills.Add(sk);
					}
					break;
				case 4:
				case 5:
				case 6:
					prop.PropertyValue2 = reader.ReadUInt32BE();
					break;
				case 7:
				case 9:
					{
						var statVals = new ushort[12];
						var statParams = new uint[12];
						for (int j = 0; j < 12; j++) statVals[j] = reader.ReadUInt16BE();
						for (int j = 0; j < 12; j++) statParams[j] = reader.ReadUInt32BE();
						prop.StatValues = statVals;
						prop.StatParams = statParams;
					}
					break;
				case 8:
					prop.PropertyValue6 = reader.ReadByte();
					break;
			}

			list.Add(prop);
		}
		return list;
	}

	private static ItemSlotData _parse_item_slot_data(PacketReader reader)
	{
		var item = new ItemSlotData();
		item.ItemId = reader.ReadUInt32BE();
		item.VisualId = reader.ReadUInt32BE();

		for (int b = 0; b < 2; b++)
		{
			var block = new ItemSlotData.StatBlock();
			block.HasStats = reader.ReadByte();
			if (block.HasStats != 0)
			{
				block.StatType = reader.ReadByte();
				for (int i = 0; i < 6; i++) block.Stats[i] = reader.ReadUInt32BE();
			}
			item.StatBlocks[b] = block;
		}

		item.DurabilityOrCount = reader.ReadUInt16BE();
		item.MagicProperties = _parse_item_magic_properties(reader);
		return item;
	}

	private static List<ItemSlotData> _parse_equipment(PacketReader reader)
	{
		var equipment = new List<ItemSlotData>();
		uint bitmask = (uint)reader.ReadInt32BE();
		for (int i = 0; i < 22; i++)
		{
			if (((bitmask >> i) & 1) != 0)
			{
				var it = _parse_item_slot_data(reader);
				it.Slot = i;
				equipment.Add(it);
			}
		}
		return equipment;
	}

	private static AppearanceBlock _parse_appearance(PacketReader reader)
	{
		var ap = new AppearanceBlock();
		ap.UnknownAppearanceFlag = reader.ReadByte();
		for (int i = 0; i < 9; i++)
		{
			var slot = new AppearanceBlock.AppearanceSlot { SlotIndex = i };
			slot.Type = reader.ReadByte();
			if (slot.Type == 1)
			{
				slot.VisualId1 = reader.ReadUInt32BE();
				slot.VisualId2 = reader.ReadUInt32BE();
			}
			else
			{
				slot.ColorOrIndex = reader.ReadByte();
			}
			ap.Slots.Add(slot);
		}

		for (int i = 0; i < 4; i++)
		{
			var prop = new AppearanceBlock.AppearanceProp { PropIndex = i };
			prop.Id = reader.ReadUInt32BE();
			prop.Type = reader.ReadByte();
			if (prop.Type == 1) prop.ValueInt = reader.ReadUInt32BE();
			else prop.ValueByte = reader.ReadByte();
			ap.Properties.Add(prop);
		}

		return ap;
	}

	private static UserStateBlock _parse_user_state(PacketReader reader)
	{
		var s = new UserStateBlock();
		s.UnknownFlag1 = reader.ReadByte();
		for (int i = 0; i < 8; i++) s.StateData[i] = reader.ReadUInt32BE();
		s.UnknownBool = reader.ReadByte() != 0;
		s.UnknownFlag2 = reader.ReadByte();
		s.UnknownDiscardedByte = reader.ReadByte();
		s.UnknownFlag3 = reader.ReadByte();
		return s;
	}
}
