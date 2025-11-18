using kakia_talesweaver_packets.Models;
using kakia_talesweaver_utils;

namespace kakia_talesweaver_packets.Packets;

public class EntitySpawnPacket
{
	// Base packet: opcode (0x07) then sub-opcode
	public byte Opcode { get; set; } = 0x07;
	public ActionType SubOpcode { get; set; }

	// If SubOpcode == Spawn, this indicates the spawn kind
	public SpawnType? SpawnKind { get; set; }

	// Variant payloads
	public RemoveObjectPayload? RemovePayload { get; set; }
	public ObjectDiePayload? DiePayload { get; set; }
	public SpawnPayload? Spawn { get; set; }

	public class RemoveObjectPayload
	{
		public int ObjectID { get; set; }
	}

	public class ObjectDiePayload
	{
		public int ObjectID { get; set; }
		public int AttackerID { get; set; }
	}

	// Base spawn payload (derived by specific spawn type)
	public abstract class SpawnPayload { }

	public class SpawnPlayerPayload : SpawnPayload
	{
		public int ObjectID { get; set; }
		public int UnknownID { get; set; }
		public int CharacterID { get; set; }
		public short PositionX { get; set; }
		public short PositionY { get; set; }
		public byte Stance { get; set; }
		public List<GameItem> EquippedItems { get; set; } = new();
		public GameItem PetInfo { get; set; }
	}

	public class SpawnNPCPayload : SpawnPayload
	{
		public int ObjectID { get; set; }
		public int? NpcID { get; set; } // present for SpawnType==2
		public int? Unknown_v3 { get; set; }
		public int Unknown_v30 { get; set; }
		public short PositionX { get; set; }
		public short PositionY { get; set; }
		public byte Stance { get; set; }
		public byte[]? RemainingData { get; set; }
	}

	public class SpawnItemPayload : SpawnPayload
	{
		public GameItem Item { get; set; } // Parsed by sub_5D0EF0
		public int OwnerID { get; set; }   // Parsed immediately after
		public short PositionX { get; set; }
		public short PositionY { get; set; }
		public short DroppedAmount { get; set; } // The amount displayed on the floor
	}

	public class SpawnPortalPayload : SpawnPayload
	{
		public int ObjectID { get; set; }
		public short PositionX { get; set; }
		public short PositionY { get; set; }
		public short TargetMapID { get; set; }
		public short TargetPortalID { get; set; }
	}

	public class SpawnReactorPayload : SpawnPayload
	{
		public int ObjectID { get; set; }
		public int ReactorID { get; set; }
		public short PositionX { get; set; }
		public short PositionY { get; set; }
		public short Unknown_v13 { get; set; }
		public short Unknown_v12 { get; set; }
		public short Unknown_v11 { get; set; }
		public short Unknown_v10 { get; set; }
		public short Unknown_v14 { get; set; }
		public short Unknown_v1 { get; set; }
		public int Unknown_v2 { get; set; }
		public byte Flag1_v22 { get; set; }
		public byte Flag2_v21 { get; set; }
		public byte Flag3_v20 { get; set; }
		public byte Flag4_v3 { get; set; }
		public byte Flag5_v4 { get; set; }
		public byte[]? RemainingData { get; set; }
	}

	public class SpawnPetPayload : SpawnPayload
	{
		public int ObjectID { get; set; }
		public int OwnerID { get; set; }
		public short PositionX { get; set; }
		public short PositionY { get; set; }
		public byte Stance { get; set; }
		public byte[]? RemainingData { get; set; }
	}

	public class SpawnSummonPayload : SpawnPayload
	{
		public byte Unknown_v33 { get; set; }
		public int ObjectID { get; set; }
		public int OwnerID { get; set; }
		public int SkillID { get; set; }
		public short PositionX { get; set; }
		public short PositionY { get; set; }
		public byte Stance { get; set; }
		public byte[]? RemainingData { get; set; }
	}

	public class SpawnMonsterPayload : SpawnPayload
	{
		public int ObjectID { get; set; }
		public int MonsterID { get; set; }
		public byte[]? RemainingData { get; set; }
	}

	/// <summary>
	/// Parse a 0x07 packet. Accepts data that either starts with 0x07 (opcode)
	/// or starts at the sub-opcode directly. Big-endian reads are used.
	/// </summary>
	public static EntitySpawnPacket FromBytes(byte[] data)
	{
		var result = new EntitySpawnPacket();
		var reader = new PacketReader(data);
		try
		{
			byte first = reader.ReadByte();
			byte subop;
			if (first == 0x07)
			{
				// next byte is subopcode
				subop = reader.ReadByte();
			}
			else
			{
				subop = first;
			}

			result.SubOpcode = (ActionType)subop;

			switch (result.SubOpcode)
			{
				case ActionType.Despawn: // remove
					var rem = new RemoveObjectPayload();
					rem.ObjectID = reader.ReadInt32BE();
					result.RemovePayload = rem;
					break;
				case ActionType.Die:
					var die = new ObjectDiePayload();
					die.ObjectID = reader.ReadInt32BE();
					die.AttackerID = reader.ReadInt32BE();
					result.DiePayload = die;
					break;
				case ActionType.Spawn:
					// read spawn type
					byte spawnType = reader.ReadByte();
					result.SpawnKind = (SpawnType)spawnType;

					switch (result.SpawnKind)
					{
						case SpawnType.Player:
							var p = new SpawnPlayerPayload();
							p.ObjectID = reader.ReadInt32BE();
							p.UnknownID = reader.ReadInt32BE();
							p.CharacterID = reader.ReadInt32BE();
							p.PositionX = (short)reader.ReadInt16BE();
							p.PositionY = (short)reader.ReadInt16BE();
							p.Stance = reader.ReadByte();
							for (int i = 0; i < 10; i++)
							{
								p.EquippedItems.Add(GameItem.Parse(reader));
							}
							p.PetInfo = GameItem.Parse(reader);
							result.Spawn = p;
							break;

						case SpawnType.MonsterNpc: // Monster/NPC basic
						case SpawnType.MonsterNpc2:
						case SpawnType.MonsterNpc3:
							var npc = new SpawnNPCPayload();
							npc.ObjectID = reader.ReadInt32BE();
							if (result.SpawnKind == SpawnType.MonsterNpc)
							{
								npc.NpcID = reader.ReadInt32BE();
								npc.Unknown_v3 = reader.ReadInt32BE();
							}
							npc.Unknown_v30 = reader.ReadInt32BE();
							npc.PositionX = (short)reader.ReadInt16BE();
							npc.PositionY = (short)reader.ReadInt16BE();
							npc.Stance = reader.ReadByte();
							int remLenN = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
							if (remLenN > 0) npc.RemainingData = reader.ReadBytes(remLenN);
							result.Spawn = npc;
							break;
						case SpawnType.Item:
							var drop = new SpawnItemPayload();
							// The parser reads exactly the right amount of bytes for the item
							drop.Item = GameItem.Parse(reader);

							// Now the reader is positioned correctly for the rest
							drop.OwnerID = reader.ReadInt32BE();
							drop.PositionX = (short)reader.ReadInt16BE();
							drop.PositionY = (short)reader.ReadInt16BE();
							drop.DroppedAmount = (short)reader.ReadInt16BE();
							result.Spawn = drop;
							break;
						case SpawnType.Portal:
							var portal = new SpawnPortalPayload();
							portal.ObjectID = reader.ReadInt32BE();
							portal.PositionX = (short)reader.ReadInt16BE();
							portal.PositionY = (short)reader.ReadInt16BE();
							portal.TargetMapID = (short)reader.ReadInt16BE();
							portal.TargetPortalID = (short)reader.ReadInt16BE();
							result.Spawn = portal;
							break;
						case SpawnType.Reactor:
							var r = new SpawnReactorPayload();
							r.ObjectID = reader.ReadInt32BE();
							r.ReactorID = reader.ReadInt32BE();
							r.PositionX = (short)reader.ReadInt16BE();
							r.PositionY = (short)reader.ReadInt16BE();
							r.Unknown_v13 = (short)reader.ReadInt16BE();
							r.Unknown_v12 = (short)reader.ReadInt16BE();
							r.Unknown_v11 = (short)reader.ReadInt16BE();
							r.Unknown_v10 = (short)reader.ReadInt16BE();
							r.Unknown_v14 = (short)reader.ReadInt16BE();
							r.Unknown_v1 = (short)reader.ReadInt16BE();
							r.Unknown_v2 = reader.ReadInt32BE();
							r.Flag1_v22 = reader.ReadByte();
							r.Flag2_v21 = reader.ReadByte();
							r.Flag3_v20 = reader.ReadByte();
							r.Flag4_v3 = reader.ReadByte();
							r.Flag5_v4 = reader.ReadByte();
							int remLenR = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
							if (remLenR > 0) r.RemainingData = reader.ReadBytes(remLenR);
							result.Spawn = r;
							break;
						case SpawnType.Pet:
							var pet = new SpawnPetPayload();
							pet.ObjectID = reader.ReadInt32BE();
							pet.OwnerID = reader.ReadInt32BE();
							pet.PositionX = (short)reader.ReadInt16BE();
							pet.PositionY = (short)reader.ReadInt16BE();
							pet.Stance = reader.ReadByte();
							int remLenPet = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
							if (remLenPet > 0) pet.RemainingData = reader.ReadBytes(remLenPet);
							result.Spawn = pet;
							break;
						case SpawnType.SummonedCreature:
							var ssum = new SpawnSummonPayload();
							ssum.Unknown_v33 = reader.ReadByte();
							ssum.ObjectID = reader.ReadInt32BE();
							ssum.OwnerID = reader.ReadInt32BE();
							ssum.SkillID = reader.ReadInt32BE();
							ssum.PositionX = (short)reader.ReadInt16BE();
							ssum.PositionY = (short)reader.ReadInt16BE();
							ssum.Stance = reader.ReadByte();
							int remLenSum = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
							if (remLenSum > 0) ssum.RemainingData = reader.ReadBytes(remLenSum);
							result.Spawn = ssum;
							break;
						case SpawnType.Unknown:
						default:
							throw new Exception("Unknown spawn type encountered");

					}
					break;
				default:
					// unknown subopcode - stop
					break;
			}
		}
		catch (Exception)
		{
			// best effort: return what we have so far
			return result;
		}

		return result;
	}

	// Serializes the packet (always emits the opcode byte first)
	public byte[] ToBytes()
	{
		using PacketWriter pw = new();

		// opcode and sub-opcode
		pw.Write(Opcode);
		pw.Write((byte)SubOpcode);

		switch (SubOpcode)
		{
			case ActionType.Despawn:
				if (RemovePayload != null)
				{
					pw.Write(RemovePayload.ObjectID);
				}
				break;
			case ActionType.Die:
				if (DiePayload != null)
				{
					pw.Write(DiePayload.ObjectID);
					pw.Write(DiePayload.AttackerID);
				}
				break;
			case ActionType.Spawn:
				// write spawn type
				pw.Write((byte)(SpawnKind ?? SpawnType.Unknown));
				if (SpawnKind == null || Spawn == null)
				{
					break;
				}

				switch (SpawnKind.Value)
				{
					case SpawnType.Player:
						var p = Spawn as SpawnPlayerPayload;
						if (p != null)
						{
							pw.Write(p.ObjectID);
							pw.Write(p.UnknownID);
							pw.Write(p.CharacterID);
							pw.Write((short)p.PositionX);
							pw.Write((short)p.PositionY);
							pw.Write(p.Stance);
							foreach (var equipItem in p.EquippedItems)
							{
								pw.Write(equipItem.ToBytes());
							}
							pw.Write(p.PetInfo.ToBytes());
							pw.Write((byte)0);
						}
						break;
					case SpawnType.MonsterNpc:
					case SpawnType.MonsterNpc2:
					case SpawnType.MonsterNpc3:
						var npc = Spawn as SpawnNPCPayload;
						if (npc != null)
						{
							pw.Write(npc.ObjectID);
							if (SpawnKind == SpawnType.MonsterNpc)
							{
								pw.Write(npc.NpcID ?? 0);
								pw.Write(npc.Unknown_v3 ?? 0);
							}
							pw.Write(npc.Unknown_v30);
							pw.Write((short)npc.PositionX);
							pw.Write((short)npc.PositionY);
							pw.Write(npc.Stance);
							if (npc.RemainingData != null) pw.Write(npc.RemainingData);
						}
						break;
					case SpawnType.Item:
						var item = Spawn as SpawnItemPayload;
						if (item != null)
						{
							// Serialize the item
							pw.Write(item.Item.ToBytes());
							// Now the rest
							pw.Write(item.OwnerID);
							pw.Write((short)item.PositionX);
							pw.Write((short)item.PositionY);
							pw.Write((short)item.DroppedAmount);
						}
						break;
					case SpawnType.Portal:
						var portal = Spawn as SpawnPortalPayload;
						if (portal != null)
						{
							pw.Write(portal.ObjectID);
							pw.Write((short)portal.PositionX);
							pw.Write((short)portal.PositionY);
							pw.Write((short)portal.TargetMapID);
							pw.Write((short)portal.TargetPortalID);
						}
						break;
					case SpawnType.Reactor:
						var r = Spawn as SpawnReactorPayload;
						if (r != null)
						{
							pw.Write(r.ObjectID);
							pw.Write(r.ReactorID);
							pw.Write((short)r.PositionX);
							pw.Write((short)r.PositionY);
							pw.Write((short)r.Unknown_v13);
							pw.Write((short)r.Unknown_v12);
							pw.Write((short)r.Unknown_v11);
							pw.Write((short)r.Unknown_v10);
							pw.Write((short)r.Unknown_v14);
							pw.Write((short)r.Unknown_v1);
							pw.Write(r.Unknown_v2);
							pw.Write(r.Flag1_v22);
							pw.Write(r.Flag2_v21);
							pw.Write(r.Flag3_v20);
							pw.Write(r.Flag4_v3);
							pw.Write(r.Flag5_v4);
							if (r.RemainingData != null) pw.Write(r.RemainingData);
						}
						break;
					case SpawnType.Pet:
						var pet = Spawn as SpawnPetPayload;
						if (pet != null)
						{
							pw.Write(pet.ObjectID);
							pw.Write(pet.OwnerID);
							pw.Write((short)pet.PositionX);
							pw.Write((short)pet.PositionY);
							pw.Write(pet.Stance);
							if (pet.RemainingData != null) pw.Write(pet.RemainingData);
						}
						break;
					case SpawnType.SummonedCreature:
						var ssum = Spawn as SpawnSummonPayload;
						if (ssum != null)
						{
							pw.Write(ssum.Unknown_v33);
							pw.Write(ssum.ObjectID);
							pw.Write(ssum.OwnerID);
							pw.Write(ssum.SkillID);
							pw.Write((short)ssum.PositionX);
							pw.Write((short)ssum.PositionY);
							pw.Write(ssum.Stance);
							if (ssum.RemainingData != null) pw.Write(ssum.RemainingData);
						}
						break;
					case SpawnType.Unknown:
					default:
						throw new Exception("Unknown spawn type encountered during serialization");
				}
				break;
			default:
				break;
		}

		return pw.ToArray();
	}
}

public enum ActionType : byte
{
	Spawn = 0x00,
	Despawn = 0x01,
	Die = 0x02
}

public enum SpawnType : byte
{
	Unknown = 0x00,
	Player = 0x01,
	MonsterNpc = 0x02,
	Item = 0x03,
	Portal = 0x04,
	Reactor = 0x05,
	Pet = 0x06,
	MonsterNpc2 = 0x07,
	SummonedCreature = 0x08,
	MonsterNpc3 = 0x09
}