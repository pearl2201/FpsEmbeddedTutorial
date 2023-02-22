using System.Collections.Generic;
using System.Linq;

using DarkRift;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

struct ReconciliationInfo
{
	public ReconciliationInfo(uint frame, PlayerStateData data, PlayerInputData input)
	{
		Frame = frame;
		Data = data;
		Input = input;
	}

	public uint Frame;
	public PlayerStateData Data;
	public PlayerInputData Input;
}

[RequireComponent(typeof(PlayerLogic))]
[RequireComponent(typeof(PlayerInterpolation))]
public class ClientPlayer : MonoBehaviour
{
	private PlayerLogic playerLogic;

	private PlayerInterpolation interpolation;

	private Queue<ReconciliationInfo> reconciliationHistory = new Queue<ReconciliationInfo>();

	// Store look direction.
	private float yaw;
	private float pitch;

	private ushort id;
	private string playerName;
	private bool isOwn;

	private int health;

	[Header("Settings")]
	[SerializeField]
	private float sensitivityX;
	[SerializeField]
	private float sensitivityY;

	[Header("HealthBar")]
	[SerializeField]
	private TMP_Text nameText;
	[SerializeField]
	private Slider healthSlider;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject shotPrefab;

	void Awake()
	{
		playerLogic = GetComponent<PlayerLogic>();
		interpolation = GetComponent<PlayerInterpolation>();
	}

	public void Initialize(ushort id, string playerName)
	{
		this.id = id;
		this.playerName = playerName;
		nameText.text = this.playerName;
		SetHealth(100);
		if (ConnectionManager.Instance.PlayerId == id)
		{
			isOwn = true;
			//Camera.main.transform.SetParent(transform);
			//Camera.main.transform.localPosition = new Vector3(0, 0, 0);
			//Camera.main.transform.localRotation = Quaternion.identity;
			interpolation.CurrentData = new PlayerStateData(this.id, 0, Vector3.zero, Quaternion.identity);
		}
	}

	public void SetHealth(int value)
	{
		health = value;
		healthSlider.value = health;
	}

	void LateUpdate()
	{
		Vector3 point = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));

	}

	void FixedUpdate()
	{
		if (isOwn)
		{
			bool[] inputs = new bool[6];
			inputs[0] = Input.GetKey(KeyCode.W);
			inputs[1] = Input.GetKey(KeyCode.A);
			inputs[2] = Input.GetKey(KeyCode.S);
			inputs[3] = Input.GetKey(KeyCode.D);
			inputs[4] = Input.GetKey(KeyCode.Space);
			inputs[5] = Input.GetMouseButton(0);

			if (inputs[5])
			{
				GameObject go = Instantiate(shotPrefab);
				go.transform.position = interpolation.CurrentData.Position;
				go.transform.rotation = transform.rotation;
				Destroy(go, 1f);
			}

			yaw += Input.GetAxis("Mouse X") * sensitivityX;
			pitch += Input.GetAxis("Mouse Y") * sensitivityY;

			Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

			PlayerInputData inputData = new PlayerInputData(inputs, rotation, FpsGameManager.Instance.LastReceivedServerTick - 1);

			//transform.position = interpolation.CurrentData.Position;
			 playerLogic.GetNextFrameData(inputData, interpolation.CurrentData);
			PlayerStateData nextStateData = new PlayerStateData()
			{
				LookDirection = transform.rotation,
				Position = transform.position
			};
			//interpolation.SetFramePosition(nextStateData);

			using (Message message = Message.Create((ushort)Tags.GamePlayerInput, inputData))
			{
				ConnectionManager.Instance.Client.SendMessage(message, SendMode.Reliable);
			}

			//reconciliationHistory.Enqueue(new ReconciliationInfo(FpsGameManager.Instance.ClientTick, nextStateData, inputData));
		}
	}

	public void OnServerDataUpdate(PlayerStateData playerStateData)
	{
		if (isOwn)
		{
			while (reconciliationHistory.Any() && reconciliationHistory.Peek().Frame < FpsGameManager.Instance.LastReceivedServerTick)
			{
				reconciliationHistory.Dequeue();
			}

			if (reconciliationHistory.Any() && reconciliationHistory.Peek().Frame == FpsGameManager.Instance.LastReceivedServerTick)
			{
				ReconciliationInfo info = reconciliationHistory.Dequeue();
				if (Vector3.Distance(info.Data.Position, playerStateData.Position) > 0.05f)
				{

					List<ReconciliationInfo> infos = reconciliationHistory.ToList();
					interpolation.CurrentData = playerStateData;
					transform.position = playerStateData.Position;
					transform.rotation = playerStateData.LookDirection;
					for (int i = 0; i < infos.Count; i++)
					{
						playerLogic.GetNextFrameData(infos[i].Input, interpolation.CurrentData);
						Physics.Simulate(Time.fixedDeltaTime);
					}
				}
			}

			
			//transform.position = playerStateData.Position;
			//transform.rotation = playerStateData.LookDirection;
		}
		else
		{
			//interpolation.SetFramePosition(playerStateData);
			transform.position = playerStateData.Position;
			transform.rotation = playerStateData.LookDirection;
		}
	}
}