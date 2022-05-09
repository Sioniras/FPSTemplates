using System;
using UnityEngine;

public class WeaponSpecification : MonoBehaviour
{
	public enum FiringOrigin
	{
		WeaponPositionAndOrientation,
		CameraPositionAndOrientation,
		PlayerPositionAndOrientation,
	}

	public enum WeaponAmmoType
	{
		AmmoType1,
		AmmoType2,
		AmmoType3,
		AmmoType4,
		AmmoType5,
		AmmoType6,
		AmmoType7,
		AmmoType8,
		Infinite,
	}

	public enum WeaponState
	{
		Idle,
		FiringStartupDelay,
		FiringCooldown,
		Reloading,
	}

	[Header("Main specification")]
	public string Name = "Gun";
	public WeaponAmmoType AmmoType = WeaponAmmoType.AmmoType1;

	[Header("Firing")]
	public bool AutomaticFire = false;
	public float FiringDelay = 0.0f;
	public float FiringCooldown = 1.0f;
	public uint AmmoCost = 1;
	public WeaponFireSpecification FiringSpecification;
	public FiringOrigin ShotOrigin = FiringOrigin.WeaponPositionAndOrientation;

	[Header("Reloading")]
	public bool EnableReloading = true;
	public bool AutoReload = true;
	public float ReloadTime = 3.0f;
	public uint ClipSize = 10;
	public bool DiscardUnusedInClip = true;

	[Header("Appearance")]
	public GameObject VisualRepresentation;
	public GameObject MuzzleFlash;
	public float MuzzleFlashDuration = 0.1f;
	public string FiringAnimation = "Fire";
	public string ReloadingAnimation = "Reload";
	public AudioSource FiringSound;
	public AudioSource PickupSound;

	public uint AmmoInClip { get; private set; } = 0;
	public WeaponState State { get; private set; } = WeaponState.Idle;

	private float firingStartupTime = float.NegativeInfinity;
	private float reloadStartupTime = float.NegativeInfinity;

	// Start is called before the first frame update
	void Start()
	{
		MuzzleFlash?.gameObject.SetActive(false);
		if (MuzzleFlashDuration > FiringCooldown)
			MuzzleFlashDuration = FiringCooldown;
	}

	// Update is called once per frame
	void Update()
	{
		if (GameController.Controller?.CurrentWeapon != this)
		{
			// Only consider currently active weapon
			State = WeaponState.Idle;
			this.gameObject.SetActive(false);
			VisualRepresentation?.SetActive(false);
		}
		else if ((GameController.Controller?.CheckReloadWeaponRequested(this) ?? false)
			&& (GameController.Controller?.CheckCanReloadWeapon(this) ?? false))
		{
			// Player is trying to fire weapon
			State = WeaponState.Reloading;
			reloadStartupTime = Time.time;

			// Play reloading animation
			VisualRepresentation?.GetComponent<Animator>()?.Play(ReloadingAnimation);
		}
		else if (GameController.Controller?.CheckFireWeaponRequested(this) ?? false)
		{
			// Player is trying to fire weapon
			State = WeaponState.FiringStartupDelay;
			firingStartupTime = Time.time;

			// Play firing animation
			VisualRepresentation?.GetComponent<Animator>()?.Play(FiringAnimation);
		}
		else if (State == WeaponState.FiringStartupDelay && (Time.time - firingStartupTime > FiringDelay))
		{
			// Is there ammo available?
			if (GameController.Controller?.CheckCanFireWeapon(this) ?? false)
			{
				// Yes, so let's fire the weapon!
				State = WeaponState.FiringCooldown;
				if (EnableReloading)
					AmmoInClip -= AmmoCost;
				else
					GameController.Controller?.AddAmmo(AmmoType, -(int)AmmoCost);

				// Show the muzzle flash
				MuzzleFlash?.gameObject.SetActive(true);

				FiringSound?.Play();

				FiringSpecification?.PlayerFireWeapon(this);
			}
			else
			{
				// There was no ammo available
				State = WeaponState.Idle;
			}
		}
		else if (State == WeaponState.FiringCooldown)
		{
			// Hide the muzzle flash after its duration
			if (Time.time - firingStartupTime > FiringDelay + MuzzleFlashDuration && (MuzzleFlash?.gameObject.activeSelf ?? false))
				MuzzleFlash?.gameObject.SetActive(false);

			// Firing process done, return to idle state
			if (Time.time - firingStartupTime > FiringDelay + FiringCooldown)
				State = WeaponState.Idle;
		}
		else if (State == WeaponState.Reloading && (Time.time - reloadStartupTime > ReloadTime))
		{
			// If the remaining ammo in the clip should not be discarded, put it back into the ammo pool
			if (!DiscardUnusedInClip)
				GameController.Controller?.AddAmmo(AmmoType, AmmoInClip);

			// Insert a new clip, or maybe just a partly filled clip if there is not enough ammo left
			AmmoInClip = Math.Min(ClipSize, GameController.Controller?.GetAmmo(AmmoType) ?? 0);
			GameController.Controller?.AddAmmo(AmmoType, -(int)AmmoInClip);
			State = WeaponState.Idle;
		}
	}
}
