using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
	#region Public fields that can be set in the editor
	/// <summary>
	/// The object to activate/deactivate.
	/// </summary>
	public GameObject Target;

	/// <summary>
	/// Whether the object should deactivate automatically after a while.
	/// </summary>
	public bool DeactivateAutomatically = false;

	/// <summary>
	/// Time in seconds that the object should remain active before deactivating automatically.
	/// </summary>
	public float AutoDeactivateDelay = 15.0f;

	/// <summary>
	/// Whether the object should activate automatically after a while.
	/// </summary>
	public bool ActivateAutomatically = false;

	/// <summary>
	/// Time in seconds that the object should remain deactive before activating automatically.
	/// </summary>
	public float AutoActivateDelay = 15.0f;

	/// <summary>
	/// If set to <c>true</c>, the GameObject with this component will be destroyed in the next frame after activation has occurred.
	/// </summary>
	public bool DestroyAfterActivation = false;

	/// <summary>
	/// If set to <c>true</c>, the GameObject with this component will be destroyed in the next frame after deactivation has occurred.
	/// </summary>
	public bool DestroyAfterDeactivation = false;

	/// <summary>
	/// If <seealso cref="DestroyAfterActivation"/> or <seealso cref="DestroyAfterDeactivation"/> is set to <c>true</c>, delay the destruction by this time (in seconds).
	/// </summary>
	public float DestructionDelay = 0;

	[Header("Triggers")]
	/// <summary>
	/// Reference to trigger that can activate or deactivate the object.
	/// </summary>
	public Trigger ActivatorTrigger;

	/// <summary>
	/// Triggers can only activate the object if <seealso cref="TriggerCanActivate"/> is true.
	/// </summary>
	public bool TriggerCanActivate = true;

	/// <summary>
	/// Triggers can only deactivate the object if <seealso cref="TriggerCanDeactivate"/> is true.
	/// </summary>
	public bool TriggerCanDeactivate = false;
	#endregion

	private float activationTime = float.NegativeInfinity;
	private float deactivationTime = float.NegativeInfinity;

	// Start is called before the first frame update
	void Start()
	{
		// Register trigger
		if (ActivatorTrigger != null)
			ActivatorTrigger.TriggerFired += On_TriggerFired;
	}

	// Triggers can activate/deactivate the object
	public void On_TriggerFired(Trigger trigger)
	{
		if ((!Target?.activeSelf ?? false) && TriggerCanActivate)
			Activate();
		else if ((Target?.activeSelf ?? false) && TriggerCanDeactivate)
			Deactivate();
	}

	// Update is called once per frame
	void Update()
	{
		if ((Target?.activeSelf ?? false) && (activationTime != float.NegativeInfinity))
		{
			// Check for auto-deactivate
			if (DeactivateAutomatically && (Time.time - activationTime > AutoDeactivateDelay))
			{
				if (ActivatorTrigger != null && ActivatorTrigger.UseAutoTrigger
					&& (GameController.Controller?.CheckForAutoInteraction(ActivatorTrigger.transform.position, ActivatorTrigger.CustomInteractionDistance, ActivatorTrigger.UseDefaultInteractionDistance) ?? false))
					activationTime += 1.0f;
				else
					Deactivate();
			}

			// Destroy GameObject after activation
			if (DestroyAfterActivation && (Time.time - activationTime > DestructionDelay))
				DestroyTarget();
		}
		else if ((!Target?.activeSelf ?? false) && (deactivationTime != float.NegativeInfinity))
		{
			// Check for auto-activate
			if (ActivateAutomatically && (Time.time - deactivationTime > AutoActivateDelay))
			{
				if (ActivatorTrigger != null && ActivatorTrigger.UseAutoTrigger
					&& (GameController.Controller?.CheckForAutoInteraction(ActivatorTrigger.transform.position, ActivatorTrigger.CustomInteractionDistance, ActivatorTrigger.UseDefaultInteractionDistance) ?? false))
					deactivationTime += 1.0f;
				else
					Activate();
			}

			// Destroy GameObject after deactivation
			if (DestroyAfterDeactivation && (Time.time - deactivationTime > DestructionDelay))
				DestroyTarget();
		}
	}

	private void DestroyTarget()
	{
		if (Target != null)
			Destroy(Target);
		Target = null;
		Destroy(this);
	}

	private void Activate()
	{
		Target?.SetActive(true);
		activationTime = Time.time;
	}

	private void Deactivate()
	{
		Target?.SetActive(false);
		deactivationTime = Time.time;
	}
}
