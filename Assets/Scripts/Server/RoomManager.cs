using System.Collections.Generic;

using DarkRift;
using DarkRift.Server;

using UnityEngine;

public class RoomManager : MonoBehaviour
{
	Dictionary<string, Room> rooms = new Dictionary<string, Room>();

	public static RoomManager Instance;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject roomPrefab;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this);
		CreateRoom("Main", 0, 25);
		//CreateRoom("Main 2", 1, 15);
	}

	public void CreateRoom(string roomName, int idx, byte maxSlots)
	{
		GameObject go = Instantiate(roomPrefab);
		go.transform.position = new Vector3(0, 0, 0);
		Room room = go.GetComponent<Room>();
		room.Initialize(roomName, maxSlots);
		rooms.Add(roomName, room);
	}

	public void RemoveRoom(string roomName)
	{
		Room r = rooms[name];
		r.Close();
		rooms.Remove(roomName);
	}

	public RoomData[] GetRoomDataList()
	{
		RoomData[] data = new RoomData[rooms.Count];
		int i = 0;
		foreach (KeyValuePair<string, Room> kvp in rooms)
		{
			Room r = kvp.Value;
			data[i] = new RoomData(r.Name, (byte)r.ClientConnections.Count, r.MaxSlots);
			i++;
		}
		return data;
	}

	public void TryJoinRoom(IClient client, JoinRoomRequest data)
	{
		bool canJoin = ServerManager.Instance.Players.TryGetValue(client.ID, out var clientConnection);

		if (!rooms.TryGetValue(data.RoomName, out var room))
		{
			canJoin = false;
		}
		else if (room.ClientConnections.Count >= room.MaxSlots)
		{
			canJoin = false;
		}

		if (canJoin)
		{
			room.AddPlayerToRoom(clientConnection);
		}
		else
		{
			using (Message m = Message.Create((ushort)Tags.LobbyJoinRoomDenied, new LobbyInfoData(GetRoomDataList())))
			{
				client.SendMessage(m, SendMode.Reliable);
			}
		}
	}
}