using kakia_talesweaver_packets.Packets;
using kakia_talesweaver_utils.Extensions;


namespace kakia_talesweaver_tests
{
	public class TestPackets
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void TestCharacterSpawn()
		{
			byte[] packet_sziadan =
				@"33 00 00 7E 46 2E 01 00 00 00 00 01 2F 00 E3 06
00 16 FF FF FF FF FF FF FF FF 00 07 53 7A 69 61
64 61 6E 00 00 00 00 00 00 00 00 00 00 01 18 00
00 00 00 00 00 01 18 00 00 00 00 00 00 00 00 00
00 00 1E 84 8D 00 00 00 0E 00 0F A6 E4 00 00 00
00 00 00 00 00 00 00 0F A1 6E 00 00 00 00 00 00
00 00 00 00 0F A1 BA 00 00 00 00 00 00 00 00 00
FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 00 00 06 00 A1 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 31 07 A6 B1 00".ToByteArray();

			byte[] packet_zapero =
				@"33 00 03 F7 6D AF 00 00 00 00 00 00 AD 01 0A 02
01 1B 00 A9 01 0B 00 BC 01 08 00 06 5A 61 70 65
72 6F 00 00 00 00 00 00 00 00 00 00 02 39 00 00
00 00 00 00 02 39 00 00 00 00 00 00 00 20 1A F9
00 1E 84 80 00 00 00 0E 00 0F A0 32 00 00 00 00
00 00 00 00 00 00 0F A1 6E 00 00 00 00 00 00 00
00 00 00 0F A1 E7 00 00 00 00 00 00 00 00 00 FF
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 00 06 00 5B 00 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 40 06 F8 84 00".ToByteArray();



			var spawn_sziadan = SpawnCharacterPacket.FromBytes(packet_sziadan);
			if (spawn_sziadan == null)
				Assert.Fail("Failed to parse SpawnCharacterPacket");

			var bytes_sziadan = spawn_sziadan?.ToBytes(true);

			if (!bytes_sziadan.SequenceEqual(packet_sziadan))			
				Assert.Fail($"Serialized bytes do not match original packet.{Environment.NewLine}Original:{Environment.NewLine}{packet_sziadan.ToFormatedHexString()}{Environment.NewLine}Serialized:{Environment.NewLine}{bytes_sziadan.ToFormatedHexString()}");			



			var spawn_zapero = SpawnCharacterPacket.FromBytes(packet_zapero);
			if (spawn_zapero == null)
				Assert.Fail("Failed to parse SpawnCharacterPacket");

			var bytes_zapero = spawn_zapero.ToBytes(false);

			if (!bytes_zapero.SequenceEqual(packet_zapero))
				Assert.Fail($"Serialized bytes do not match original packet.{Environment.NewLine}Original:{Environment.NewLine}{packet_zapero.ToFormatedHexString()}{Environment.NewLine}Serialized:{Environment.NewLine}{bytes_zapero.ToFormatedHexString()}");
			else
				Assert.Pass();
		}

		[Test]
		public void TestSpawnPacket()
		{
			byte[] packetMob = @"07 00 02 2D DE EE 02 00 00 00 00 00 00 00 00 00
20 0F 95 02 5E 00 E1 00 00 08 02 5E 00 E1 02 5E
00 E1 00 00 00 00 00 00 01 8E 00 00 00 00 00 00
01 8E 00 07 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 ".ToByteArray();

			var spawnMobPacket = EntitySpawnPacket.FromBytes(packetMob);
			if (spawnMobPacket == null)
				Assert.Fail("Failed to parse SpawnObjectPacket");
			var bytesMob = spawnMobPacket.ToBytes();
			if (!bytesMob.SequenceEqual(packetMob))
				Assert.Fail($"Serialized bytes do not match original packet.{Environment.NewLine}Original:{Environment.NewLine}{packetMob.ToFormatedHexString()}{Environment.NewLine}Serialized:{Environment.NewLine}{bytesMob.ToFormatedHexString()}");



			byte[] packetPlayer = @"07 00 01 DE AA 02 00 00 00 00 00 00 21 9B 6D 04
3A 01 FD 06 00 0A FF FF FF FF FF FF FF FF 00 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 00 00 00 00 ".ToByteArray();

			var spawnPlayerPacket = EntitySpawnPacket.FromBytes(packetPlayer);
			if (spawnPlayerPacket == null)
				Assert.Fail("Failed to parse SpawnObjectPacket");
			var bytesPlayer = spawnPlayerPacket.ToBytes();
			if (!bytesPlayer.SequenceEqual(packetPlayer))
				Assert.Fail($"Serialized bytes do not match original packet.{Environment.NewLine}Original:{Environment.NewLine}{packetPlayer.ToFormatedHexString()}{Environment.NewLine}Serialized:{Environment.NewLine}{bytesPlayer.ToFormatedHexString()}");



			byte[] packetItem = @"07 00 03 75 F6 48 38 00 12 83 8C 83 43 83 93 83
7B 81 5B 83 7D 83 4A 83 8D 83 93 00 00 00 00 03
FB 02 B4 00 0F AD 75 00 00 01 00 01 00 00 00".ToByteArray();

			var spawnItemPacket = EntitySpawnPacket.FromBytes(packetItem);
			if (spawnItemPacket == null)
				Assert.Fail("Failed to parse SpawnObjectPacket");
			var bytesItem = spawnItemPacket.ToBytes();
			if (!bytesItem.SequenceEqual(packetItem))
				Assert.Fail($"Serialized bytes do not match original packet.{Environment.NewLine}Original:{Environment.NewLine}{packetItem.ToFormatedHexString()}{Environment.NewLine}Serialized:{Environment.NewLine}{bytesItem.ToFormatedHexString()}");



			Assert.Pass();
		}
	}
}
