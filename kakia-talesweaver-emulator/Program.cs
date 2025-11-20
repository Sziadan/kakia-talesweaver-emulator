using kakia_talesweaver_emulator.Models;
using kakia_talesweaver_emulator.Network;
using kakia_talesweaver_emulator.PacketHandlers;
using kakia_talesweaver_logging;
using kakia_talesweaver_utils.Extensions;
using System.Collections.Concurrent;
using System.Text;

PacketHandlers.LoadPacketHandlers("kakia_talesweaver_emulator.PacketHandlers");
Logger.SetLogLevel(LogLevel.Debug);

ConcurrentDictionary<uint, SessionInfo> accountSessions  = new();

string listenIp = "127.0.0.1";

TalesServer loginServer = new(listenIp, 20000, ServerType.Login, accountSessions);
TalesServer lobbyServer = new(listenIp, 20001, ServerType.Lobby, accountSessions);
TalesServer worldServer = new(listenIp, 20002, ServerType.World, accountSessions);

CancellationTokenSource ct = new();

List<Task> serverTasks =
[
	loginServer.Run(ct.Token),
	lobbyServer.Run(ct.Token),
	worldServer.Run(ct.Token)
];


Logger.Log("==== [Server is now running, paste hex packets and press Enter on an empty line to send, or enter 'q' alone to quit] ====", LogLevel.Information);

// Interactive paste loop: accept multiple lines of hex, send when an empty line is entered.
while (true)
{
	Console.WriteLine("Paste packet hex lines. Enter an empty line to send, or 'q' on its own line to quit.");
	var sb = new StringBuilder();
	while (true)
	{
		var line = Console.ReadLine();
		if (line == null)
			break;
		var trimmed = line.Trim();
		if (trimmed.Equals("q", StringComparison.OrdinalIgnoreCase))
			goto Quit;
		if (string.IsNullOrEmpty(trimmed))
			break; // end of current packet
		sb.Append(trimmed).Append(' ');
	}

	var input = sb.ToString().Trim();
	if (!string.IsNullOrEmpty(input))
	{
		SendInputPacket(input);
		//Console.Clear();
	}
}

Quit:
ct.Cancel();
Task.WaitAll(serverTasks);


void SendInputPacket(string input)
{
	var packet = input.ToByteArray();
	foreach(var p in loginServer.ConnectedPlayers.Values)
		TrySend(p, packet);

	foreach (var p in lobbyServer.ConnectedPlayers.Values)
		TrySend(p, packet);

	foreach (var p in worldServer.ConnectedPlayers.Values)
		TrySend(p, packet);
}

void TrySend(IPlayerClient pc, byte[] packet)
{
	try	{ pc.Send(packet, CancellationToken.None).Wait(); }
	catch (Exception ex) {	}
}