namespace kakia_talesweaver_packets;

public enum PacketType : byte
{
	Chat = 0x0E,
	ClientReconnect = 0x10,
	UpdateDirection = 0x11,

	// unk
	Attack = 0x13,

	InputtedSecurityCode = 0x18,
	Heartbeat = 0x24,

	// Character creation
	CheckName = 0x28,
	UpdateCharacterInfo = 0x2A,
	CreateCharacter = 0x2C,

	SelectCharacter = 0x2B,
	Movement = 0x33,
	ClickedEntity = 0x43,
	Login = 0x66,
	ServerSelect = 0x67
}
