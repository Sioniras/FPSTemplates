using UnityEngine;

public abstract class WeaponFireSpecification : MonoBehaviour
{
	// The actual firing method that can be used by player, AIs, traps, anything...
	public abstract void FireWeapon(Vector3 origin, Vector3 direction);

	public void PlayerFireWeapon(WeaponSpecification weapon)
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

		FireWeapon(start, direction);
	}
}
