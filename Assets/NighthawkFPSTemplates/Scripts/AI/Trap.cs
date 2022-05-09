using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
	#region Public fields that can be set in the editor
	[Header("Basic Setup")]
	/// <summary>
	/// The action that happens when the trap is triggered.
	/// </summary>
	public WeaponFireSpecification TrapAction;

	/// <summary>
	/// Position offset for the trap fire action.
	/// </summary>
	public Vector3 TrapOffset = Vector3.zero;

	[Header("Periodic Trap")]
	/// <summary>
	/// If set to <c>true</c>, the <see cref="TrapAction"/> will be fired periodically.
	/// </summary>
	public bool IsPeriodic = false;

	/// <summary>
	/// Time between two trap actions.
	/// </summary>
	public float Period = 5.0f;

	/// <summary>
	/// The periodic firing of the trap is suspended when the player is closer to the trap than <see cref="ClosestDistance"/>.
	/// </summary>
	public float ClosestDistance = 0.0f;

	/// <summary>
	/// The periodic firing of the trap is suspended when the player is farther away than <see cref="MaxDistance"/>.
	/// </summary>
	public float MaxDistance = 50.0f;

	[Header("Triggers")]
	/// <summary>
	/// Reference to trigger that can activate the trap.
	/// </summary>
	public Trigger TrapTrigger;

	/// <summary>
	/// The trap can only be triggered again when this amount of time has passed since it was last triggered.
	/// </summary>
	public float TriggerCooldown = 0;
	#endregion

	private float lastTrigger = float.NegativeInfinity;
	private float lastPeriodicAction = float.NegativeInfinity;

	// Start is called before the first frame update
	void Start()
	{
		// Register trigger
		if (TrapTrigger != null)
			TrapTrigger.TriggerFired += On_TriggerFired;
	}

	// Update is called once per frame
	void Update()
	{
		if (IsPeriodic && (lastPeriodicAction + Period < Time.time))
		{
			float distance = (MaxDistance + ClosestDistance) / 2.0f;

			if(GameController.Controller != null && GameController.Controller.Player != null)
				distance = (transform.position - GameController.Controller.Player.transform.position).magnitude;

			if (distance > ClosestDistance || distance < MaxDistance)
			{
				lastPeriodicAction = Time.time;
				FireTrap();
			}
		}
	}

	// Triggers can fire the trap action
	public void On_TriggerFired(Trigger trigger)
	{
		if (lastTrigger < 0 || (lastTrigger + TriggerCooldown < Time.time))
		{
			lastTrigger = Time.time;
			FireTrap();
		}
	}

	/// <summary>
	/// Fires the trap.
	/// </summary>
	public void FireTrap()
	{
		var offset = transform.TransformVector(TrapOffset);

		// Uses position and rotation of the GameObject, but the position can be offset
		TrapAction?.FireWeapon(transform.position + offset, transform.TransformDirection(Vector3.forward));
	}
}
