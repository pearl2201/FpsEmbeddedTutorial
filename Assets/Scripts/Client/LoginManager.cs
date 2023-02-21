
using System;

using Assets.Scripts.Share;

using DarkRift;
using DarkRift.Client;

using TMPro;


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private GameObject loginWindow;
	[SerializeField]
	private TMP_InputField nameInput;
	[SerializeField]
	private Button submitLoginButton;


	void Start()
	{
		loginWindow.SetActive(false);
		ConnectionManager.Instance.OnConnected += StartLoginProcess;
		submitLoginButton.onClick.AddListener(OnSubmitLogin);

		ConnectionManager.Instance.Client.MessageReceived += OnMessage;
	}

	public void StartLoginProcess()
	{
		loginWindow.SetActive(true);
	}


	void OnDestroy()
	{
		ConnectionManager.Instance.Client.MessageReceived -= OnMessage;
		ConnectionManager.Instance.OnConnected -= StartLoginProcess;
	}
	private void OnMessage(object sender, MessageReceivedEventArgs e)
	{
		using (Message message = e.GetMessage())
		{
			switch ((Tags)message.Tag)
			{
				case Tags.LoginRequestDenied:
					OnLoginDecline();
					break;
				case Tags.LoginRequestAccepted:
					OnLoginAccept(message.Deserialize<LoginInfoData>());
					break;
			}
		}
	}

	private void OnLoginDecline()
	{
		loginWindow.SetActive(true);
	}

	private void OnLoginAccept(LoginInfoData data)
	{
		ConnectionManager.Instance.PlayerId = data.Id;
		ConnectionManager.Instance.LobbyInfoData = data.Data;
		SceneManager.LoadScene("Lobby");
	}
	public void OnSubmitLogin()
	{
		if (!String.IsNullOrEmpty(nameInput.text))
		{
			loginWindow.SetActive(false);

			using (Message message = Message.Create((ushort)Tags.LoginRequest, new LoginRequestData(nameInput.text)))
			{
				ConnectionManager.Instance.Client.SendMessage(message, SendMode.Reliable);
			}
		}
	}
}
