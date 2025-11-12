namespace kakia_talesweaver_packets;

public enum PacketType : byte
{
	Chat = 0x0E,
	ClientReconnect = 0x10,
	InputtedSecurityCode = 0x18,
	Heartbeat = 0x24,
	SelectCharacter = 0x2B,
	Movement = 0x33,
	Login = 0x66,
	ServerSelect = 0x67
}
