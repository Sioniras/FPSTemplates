using UnityEngine;

public class PhysicsBodyWeaponFireSpecification : WeaponFireSpecification
{
	/// <summary>
	/// The object to shoot from the weapon.
	/// </summary>
	public GameObject Projectile;

	/// <summary>
	/// The initial velocity of the projectile.
	/// </summary>
	public float ProjectileVelocity = 10.0f;

	public override void FireWeapon(WeaponSpecification weapon)
	{
		// Get the transform for the weapon (the point where the shot should originate)
		var origin = weapon.VisualRepresentation.transform;
		if (weapon.ShotOrigin == WeaponSpecification.FiringOrigin.PlayerPositionAndOrientation)
			origin = GameController.Controller?.Player?.transform;
		else if (weapon.ShotOrigin == WeaponSpecification.FiringOrigin.CameraPositionAndOrientation)
			origin = GameController.Controller?.Player?.GetComponentInChildren<Camera>()?.gameObject.transform;

		// Get initial position and velocity
		Vector3 start = origin.position;
		Vector3 velocity = origin.TransformDirection(Vector3.forward).normalized * ProjectileVelocity;

		// Make a copy of the projectile, set it active and place it at the weapon/shooting position
		var projectile = GameObject.Instantiate(Projectile);
		projectile.SetActive(true);
		projectile.transform.position = start;

		// Get a rigid body on the new projectile
		var rb = projectile.GetComponent<Rigidbody>();
		if (rb == null)
			rb = projectile.AddComponent<Rigidbody>();

		// Set the velocity of the projectile
		rb.AddForce(velocity, ForceMode.VelocityChange);
	}
}
