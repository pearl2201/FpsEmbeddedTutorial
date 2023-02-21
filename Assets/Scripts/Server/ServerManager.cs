using System.Collections.Generic;

using Assets.Scripts.Share;

using DarkRift;
using DarkRift.Server;
using DarkRift.Server.Unity;

using UnityEngine;
public class ServerManager : MonoBehaviour
{
	public static ServerManager Instance;

	private XmlUnityServer xmlServer;
	private DarkRiftServer server;
	public Dictionary<ushort, ClientConnection> Players = new Dictionary<ushort, ClientConnection>();
	public Dictionary<string, ClientConnection> PlayersByName = new Dictionary<string, ClientConnection>();
	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this);
	}
	void Start()
	{
		xmlServer = GetComponent<XmlUnityServer>();
		server = xmlServer.Server;
		server.ClientManager.ClientConnected += OnClientConnected;
		server.ClientManager.ClientDisconnected += OnClientDisconnected;
	}

	void OnDestroy()
	{
		server.ClientManager.ClientConnected -= OnClientConnected;
		server.ClientManager.ClientDisconnected -= OnClientDisconnected;
	}

	private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
	{
		IClient client = e.Client;
		ClientConnection p;
		if (Players.TryGetValue(client.ID, out p))
		{
			p.OnClientDisconnect(sender, e);
		}
		e.Client.MessageReceived -= OnMessage;
	}

	private void OnClientConnected(object sender, ClientConnectedEventArgs e)
	{
		e.Client.MessageReceived += OnMessage;
	}

	private void OnMessage(object sender, MessageReceivedEventArgs e)
	{
		IClient client = (IClient)sender;
		using (Message message = e.GetMessage())
		{
			switch ((Tags)message.Tag)
			{
				case Tags.LoginRequest:
					OnclientLogin(client, message.Deserialize<LoginRequestData>());
					break;
			}
		}
	}

	private void OnclientLogin(IClient client, LoginRequestData data)
	{
		// Check if player is already logged in (name already chosen in our case) and if not create a new object to represent a logged in client.
		if (PlayersByName.ContainsKey(data.Name))
		{
			using (Message message = Message.CreateEmpty((ushort)Tags.LoginRequestDenied))
			{
				client.SendMessage(message, SendMode.Reliable);
			}
			return;
		}

		// In the future the ClientConnection will handle its messages
		client.MessageReceived -= OnMessage;

		new ClientConnection(client, data);
	}
}
