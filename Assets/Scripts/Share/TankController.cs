using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


public class TankController : MonoBehaviour
{
	[SerializeField] private Rigidbody rb;

	private void Start()
	{
		//rb.angularVelocity = Vector3.zero;
		//rb.useGravity = false;
	}
	public void Move(Vector3 movement)
	{

		//var prevPos = transform.localPosition;
		//var nextPos = transform.localPosition + movement;
		//transform.localPosition = nextPos;
		//rb.MovePosition(rb.position + movement);
		//Debug.Log("Movement: " + movement.ToString() + ", prevPos: " + prevPos.ToString() + ", nextPos: " + nextPos.ToString());
		rb.velocity = movement;

		if (movement.x != 0 || movement.z != 0)
		{
			transform.rotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, movement, Vector3.up), 0);
		}

	}
}

