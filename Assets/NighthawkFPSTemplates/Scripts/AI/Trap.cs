using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
	#region Public fields that can be set in the editor
	[Header("Action")]
	/// <summary>
	/// The action that happens when the trap is triggered.
	/// </summary>
	public WeaponFireSpecification TrapAction;

	/// <summary>
	/// Position offset for the trap fire action.
	/// </summary>
	public Vector3 TrapOffset = Vector3.zero;

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

	// Start is called before the first frame update
	void Start()
	{
		// Register trigger
		if (TrapTrigger != null)
			TrapTrigger.TriggerFired += On_TriggerFired;
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
		// Uses position and rotation of the GameObject, but the position can be offset
		TrapAction?.FireWeapon(transform.position + TrapOffset, transform.TransformDirection(Vector3.forward));
	}
}
