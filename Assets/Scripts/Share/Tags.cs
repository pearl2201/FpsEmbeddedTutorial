using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tags
{
	LoginRequest = 0,
	LoginRequestAccepted = 1,
	LoginRequestDenied = 2,
	LobbyJoinRoomRequest = 100,
	LobbyJoinRoomDenied = 101,
	LobbyJoinRoomAccepted = 102,
	GameJoinRequest = 200,
	GameStartDataResponse = 201,
	GameUpdate = 202,
	GamePlayerInput = 203,
}
