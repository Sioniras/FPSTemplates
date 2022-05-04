using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	private enum AIState
	{
		Idle,
		Following,
	}

	[Header("Movement")]
	/// <summary>
	/// How fast the entity can move towards the player.
	/// </summary>
	public float movementSpeed = 1.0f;

	/// <summary>
	/// Fall speed for "fake/constant velocity" gravity (not using a proper acceleration)
	/// </summary>
	public float fallSpeed = 10.0f;

	/// <summary>
	/// Stop moving towards player when within this distance.
	/// </summary>
	public float ClosestDistance = 2f;

	/// <summary>
	/// Do not move towards the player if the player too far away (beyond this distance).
	/// </summary>
	public float MaxDistance = 10.0f;

	/// <summary>
	/// The RigidBody used to move the entity.
	/// </summary>
	public Rigidbody RigidBody;

	private AIState state = AIState.Idle;

	// Start is called before the first frame update
	void Start()
	{
		if(RigidBody == null)
		{
			RigidBody = GetComponent<Rigidbody>();
			if(RigidBody == null)
				RigidBody = gameObject.AddComponent<Rigidbody>();
		}
	}

	// Use FixedUpdate rather than Update when dealing with physics
	void FixedUpdate()
	{
		if (GameController.Controller == null || GameController.Controller.Player == null)
			return;

		var distance = (transform.position - GameController.Controller.Player.transform.position).magnitude;

		state = (distance < ClosestDistance || distance > MaxDistance) ? AIState.Idle : AIState.Following;

		switch (state)
		{
			case AIState.Following:
				transform.LookAt(GameController.Controller.Player.transform);
				var planeNewVel = transform.TransformDirection(Vector3.forward);
				var planeOldVel = RigidBody.velocity;
				planeNewVel.y = 0;
				planeOldVel.y = 0;
				var velocity = (planeNewVel.normalized * movementSpeed - planeOldVel);
				RigidBody.AddForce(velocity.normalized * movementSpeed, ForceMode.Acceleration);
				break;
			case AIState.Idle:
			default:
				// When idle, wait for player to get close
				break;
		}
	}
}
