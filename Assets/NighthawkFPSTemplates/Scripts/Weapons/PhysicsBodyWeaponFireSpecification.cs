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

	public override void FireWeapon(Vector3 origin, Vector3 direction)
	{
		// Get initial velocity
		Vector3 velocity = direction.normalized * ProjectileVelocity;

		// Make a copy of the projectile, set it active and place it at the weapon/shooting position
		var projectile = GameObject.Instantiate(Projectile);
		projectile.SetActive(true);
		projectile.transform.position = origin;

		// Get a rigid body on the new projectile
		var rb = projectile.GetComponent<Rigidbody>();
		if (rb == null)
			rb = projectile.AddComponent<Rigidbody>();

		// Set the velocity of the projectile
		rb.AddForce(velocity, ForceMode.VelocityChange);
	}
}
