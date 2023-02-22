using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TestMovement : MonoBehaviour
{
	[SerializeField] private Rigidbody rb;
	public float walkSpeed;
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		bool[] inputs = new bool[6];
		var w = Input.GetKey(KeyCode.W);
		var a = Input.GetKey(KeyCode.A);
		var s = Input.GetKey(KeyCode.S);
		var d = Input.GetKey(KeyCode.D);

		Vector3 movement = Vector3.zero;

		if (w)
		{
			movement += Vector3.forward;
		}
		if (a)
		{
			movement += Vector3.left;
		}
		if (s)
		{
			movement += Vector3.back;
		}
		if (d)
		{
			movement += Vector3.right;
		}

		movement.Normalize();
		movement = movement * walkSpeed;

		rb.velocity = movement;

		if (movement.x != 0 || movement.z != 0)
		{
			transform.rotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, movement, Vector3.up), 0);
		}
	}
}
