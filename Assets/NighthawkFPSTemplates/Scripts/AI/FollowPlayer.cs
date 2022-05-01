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

	private AIState state = AIState.Idle;
	private CharacterController controller;

	// Start is called before the first frame update
	void Start()
	{
		controller = GetComponent<CharacterController>();
		if (controller == null)
			controller = this.gameObject.AddComponent<CharacterController>();
	}

	// Update is called once per frame
	void Update()
	{
		if (GameController.Controller == null || GameController.Controller.Player == null)
			return;

		var distance = (transform.position - GameController.Controller.Player.transform.position).magnitude;

		state = (distance < ClosestDistance || distance > MaxDistance) ? AIState.Idle : AIState.Following;

		switch (state)
		{
			case AIState.Following:
				transform.LookAt(GameController.Controller.Player.transform);
				var velocity = transform.TransformDirection(Vector3.forward).normalized * movementSpeed * Time.deltaTime;
				controller.Move(velocity);
				break;
			case AIState.Idle:
			default:
				// When idle, wait for player to get close
				break;
		}

		// Fake gravity - note that the fall speed is constant here... Physically wrong, but the player probably will not notice/care anyway
		controller.Move(Vector3.down * fallSpeed * Time.deltaTime);
	}
}
