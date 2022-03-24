using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastWeaponFireSpecification : WeaponFireSpecification
{

	public float Range = 200;
	public float ForceMagnitude = 5.0f;

	[Header("Debugging")]
	public bool ShowDebugLine = false;
	public float DebugLineDuration = 1.0f;
	public Color DebugLineHitColor = Color.green;
	public Color DebugLineMissColor = Color.red;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public override void FireWeapon(WeaponSpecification weapon)
	{
		var origin = weapon.VisualRepresentation.transform;
		if (weapon.ShotOrigin == WeaponSpecification.FiringOrigin.PlayerPositionAndOrientation)
			origin = GameController.Controller?.Player?.transform;
		else if (weapon.ShotOrigin == WeaponSpecification.FiringOrigin.CameraPositionAndOrientation)
			origin = GameController.Controller?.Player?.GetComponentInChildren<Camera>()?.gameObject.transform;

		Vector3 start = origin.position;
		Vector3 direction = origin.TransformDirection(Vector3.forward);

		if (Physics.Raycast(start, direction, out RaycastHit hitInfo, Range))
		{
			if (ShowDebugLine)
				Debug.DrawLine(start, hitInfo.point, DebugLineHitColor, DebugLineDuration, true);

			// Add a force to physics objects
			var hitRigidBody = hitInfo.collider.GetComponent<Rigidbody>();
			if(hitRigidBody != null)
				hitRigidBody?.AddForceAtPosition(direction.normalized * ForceMagnitude, hitInfo.point, ForceMode.Impulse);
		}
		else
		{
			if(ShowDebugLine)
				Debug.DrawLine(start, start + direction.normalized * Range, DebugLineMissColor, DebugLineDuration, true);
		}
	}
}