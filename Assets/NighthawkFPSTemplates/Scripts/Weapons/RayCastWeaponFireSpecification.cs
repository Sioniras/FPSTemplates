using UnityEngine;

public class RayCastWeaponFireSpecification : WeaponFireSpecification
{
	/// <summary>
	/// Range of the weapon.
	/// </summary>
	public float Range = 200;

	/// <summary>
	/// Force that it hits physics objects with.
	/// </summary>
	public float ForceMagnitude = 5.0f;

	[Header("Debugging")]
	public bool LogHitTarget = false;
	public bool ShowDebugLine = false;
	public float DebugLineDuration = 1.0f;
	public Color DebugLineHitColor = Color.green;
	public Color DebugLineMissColor = Color.red;

	public override void FireWeapon(WeaponSpecification weapon)
	{
		// Get the transform for the weapon (the point where the shot should originate)
		var origin = weapon.VisualRepresentation.transform;
		if (weapon.ShotOrigin == WeaponSpecification.FiringOrigin.PlayerPositionAndOrientation)
			origin = GameController.Controller?.Player?.transform;
		else if (weapon.ShotOrigin == WeaponSpecification.FiringOrigin.CameraPositionAndOrientation)
			origin = GameController.Controller?.Player?.GetComponentInChildren<Camera>()?.gameObject.transform;

		// Get initial position and direction of the raycast
		Vector3 start = origin.position;
		Vector3 direction = origin.TransformDirection(Vector3.forward);

		// Perform the raycast to see if anything was hit
		if (Physics.Raycast(start, direction, out RaycastHit hitInfo, Range))
		{
			// Notify about what was hit
			if (LogHitTarget)
				Debug.Log("Hit target: " + hitInfo.collider.name);

			// Show debugging line if enabled (requires gizmos to be enabled in playmode)
			if (ShowDebugLine)
				Debug.DrawLine(start, hitInfo.point, DebugLineHitColor, DebugLineDuration, true);

			// Add a force to physics objects
			var hitRigidBody = hitInfo.collider.GetComponent<Rigidbody>();
			if(hitRigidBody != null)
				hitRigidBody.AddForceAtPosition(direction.normalized * ForceMagnitude, hitInfo.point, ForceMode.Impulse);

			// Activate triggers
			var trigger = hitInfo.collider.GetComponent<Trigger>();
			if(trigger != null && trigger.TriggerByWeaponFire)
				trigger.FireTrigger();
		}
		else
		{
			// Nothing was hit...

			// Show debugging line if enabled (requires gizmos to be enabled in playmode)
			if (ShowDebugLine)
				Debug.DrawLine(start, start + direction.normalized * Range, DebugLineMissColor, DebugLineDuration, true);
		}
	}
}
