using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class RoomListObject : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private TMP_Text nameText;
	[SerializeField]
	private TMP_Text slotsText;
	[SerializeField]
	private Button joinButton;

	public void Set(LobbyManager lobbyManager, RoomData data)
	{
		nameText.text = data.Name;
		slotsText.text = data.Slots + "/" + data.MaxSlots;
		joinButton.onClick.RemoveAllListeners();
		joinButton.onClick.AddListener(delegate { lobbyManager.SendJoinRoomRequest(data.Name); });
	}
}