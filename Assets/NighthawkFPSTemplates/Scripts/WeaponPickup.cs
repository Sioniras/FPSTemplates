using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
	[Header("Weapon")]
	/// <summary>
	/// Weapon to pickup, or null if no weapon is provided (only ammo box)
	/// </summary>
	public WeaponSpecification WeaponType;

	[Header("Ammo")]
	/// <summary>
	/// Ammo type to pickup.
	/// </summary>
	public WeaponSpecification.WeaponAmmoType AmmoType;

	/// <summary>
	/// Amount of ammo to pickup.
	/// </summary>
	public uint AmmoAmount = 10;

	[Header("Interactions")]
	/// <summary>
	/// Whether the player should be able to pickup the key using the <seealso cref="GameController.InteractionKey"/>.
	/// </summary>
	public bool UseInteractionKey = false;

	/// <summary>
	/// Whether the player should be able to pickup the key by getting close to it.
	/// </summary>
	public bool UseAutoPickup = true;

	/// <summary>
	/// A custom interaction distance that is used only if <seealso cref="UseDefaultInteractionDistance"/> is set to false.
	/// </summary>
	public float CustomInteractionDistance = 2.5f;

	/// <summary>
	/// Interaction distance is set to <seealso cref="GameController.InteractionDistance"/> if this is true, otherwise its set to <seealso cref="CustomInteractionDistance"/>.
	/// </summary>
	public bool UseDefaultInteractionDistance = true;

	// Update is called once per frame
	void Update()
	{
		// Check whether the weapon can be picked up
		if ((UseInteractionKey && (GameController.Controller?.CheckForInteraction(transform.position, CustomInteractionDistance, UseDefaultInteractionDistance) ?? false))
			|| (UseAutoPickup && (GameController.Controller?.CheckForAutoInteraction(transform.position, CustomInteractionDistance, UseDefaultInteractionDistance) ?? false)))
		{
			// Register that the player found the weapon
			try
			{
				// Pickup the weapon
				if(WeaponType != null)
					GameController.Controller?.SetHasWeapon(WeaponType, true);

				// Add ammo for the weapon, or for the given ammo type if no weapon was specified
				if(WeaponType != null)
				{
					GameController.Controller?.AddAmmo(WeaponType.AmmoType, AmmoAmount);
					GameController.DisplayMessage("Found " + WeaponType.Name);
				}
				else
				{
					GameController.Controller?.AddAmmo(AmmoType, AmmoAmount);
					GameController.DisplayMessage("Found " + GameController.Controller.AmmoTypeNames[(int)AmmoType]);
				}
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}

			// Then remove it
			Destroy(this.gameObject);
		}
	}
}
