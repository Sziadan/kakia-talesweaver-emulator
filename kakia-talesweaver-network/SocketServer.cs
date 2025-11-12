using kakia_talesweaver_logging;
using System.Net;
using System.Net.Sockets;

namespace kakia_talesweaver_network;

public abstract class SocketServer
{
	private Socket _server { get; set; }
	private string _host;
	private int _port;

	public SocketServer(string host, int port)
	{
		_host = host;
		_port = port;
		_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	}

	public async Task Run(CancellationToken token)
	{
		try
		{
			_server.Bind(new IPEndPoint(IPAddress.Parse(_host), _port));
			_server.Listen(0xFFFF);

			Logger.Log($"Server is listening on {_host}:{_port}");

			while (!token.IsCancellationRequested)
			{
				var sock = await _server.AcceptAsync();
				OnConnect(new SocketClient(sock));
			}
		}
		catch (Exception ex)
		{
			Logger.Log(ex);
		}
	}

	public virtual void OnConnect(SocketClient s) { }
}
