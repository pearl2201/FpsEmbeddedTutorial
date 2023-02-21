using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

using DarkRift;
using DarkRift.Client.Unity;

using UnityEngine;

[RequireComponent(typeof(UnityClient))]
public class ConnectionManager : MonoBehaviour
{
	public static ConnectionManager Instance;

	[Header("Settings")]
	[SerializeField]
	private string ipAdress;
	[SerializeField]
	private int port;

	public UnityClient Client { get; private set; }

	public delegate void OnConnectedDelegate();
	public event OnConnectedDelegate OnConnected;

	public ushort PlayerId { get; set; }

	public LobbyInfoData LobbyInfoData { get; set; }
	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(this);

		Client = GetComponent<UnityClient>();
	}

	void Start()
	{
		Client.ConnectInBackground(IPAddress.Parse(ipAdress), port, IPVersion.IPv4, ConnectCallback);
	}

	private void ConnectCallback(Exception exception)
	{
		if (Client.Connected)
		{
			//here we will add login code later
			OnConnected?.Invoke();
		}
		else
		{
			Debug.LogError("Unable to connect to server.");
		}
	}
}
