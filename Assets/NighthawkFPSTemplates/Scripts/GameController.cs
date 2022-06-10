using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
	// Let other scripts reference the controller
	public static GameController Controller { get; private set; }

	[Header("Player")]
	public GameObject Player;
	public bool PlayerCanDie = false;
	public float PlayerMaxHealth = 100.0f;
	public GameOverPanel GameOverPanel;

	[Header("Interaction Setup")]
	public KeyCode InteractionKey = KeyCode.E;
	public float InteractionDistance = 2.5f;
	public float AutoInteractionDistance = 2.0f;
	public float InteractionArcDegrees = 120.0f;

	[Header("Keys")]
	public string[] KeyNames = { "Red Key", "Green Key", "Blue Key", "Yellow Key", "Purple Key", "Golden Key", "Key Code", "Access Card" };
	public bool[] HasKey = new bool[8];

	[Header("Weapons")]
	public KeyCode FireKey = KeyCode.Mouse0;
	public KeyCode ReloadKey = KeyCode.R;
	public KeyCode[] WeaponKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8 };
	public WeaponSpecification[] Weapons = new WeaponSpecification[8];
	public string[] AmmoTypeNames = { "Bullets", "Heavy Bullets", "Shells", "Power Cells", "Plasma Cells", "Slugs", "Rockets", "Nukes" };
	public bool[] HasWeapon = new bool[8];
	public uint[] Ammo = new uint[8];
	public uint[] MaxAmmo = { 500, 250, 100, 200, 200, 50, 25, 25 };
	public WeaponSpecification SelectedAtStartup;

	[Header("Messages")]
	public MessageField MessageFieldText;

	[Header("Sounds")]
	public AudioSource[] KeyPickupSound = new AudioSource[8];
	public AudioSource[] AmmoPickupSound = new AudioSource[8];

	// Properties
	public WeaponSpecification CurrentWeapon { get; private set; }

	public CombatEntity PlayerCombatEntity { get; private set; }

	public bool PlayerHasDied { get; private set; }

	// Start is called before the first frame update
	void Start()
	{
		Controller = this;
		SetActiveWeapon(SelectedAtStartup);

		if(Player != null && PlayerCanDie)
		{
			PlayerCombatEntity = Player.GetComponent<CombatEntity>();
			if (PlayerCombatEntity == null)
				PlayerCombatEntity = Player.AddComponent<CombatEntity>();
		}
	}

	// Update is called once per frame
	void Update()
	{
		// Check whether the player wants to select a different weapon
		CheckSelectWeapon();

		// Check whether the player has died
		if(PlayerCanDie && !PlayerHasDied && PlayerCombatEntity != null && PlayerCombatEntity.IsDead)
			Die();
	}

	#region Messages
	/// <summary>
	/// Displays a message to the user.
	/// </summary>
	/// <param name="message">The message to show.</param>
	/// <param name="displayTime">Time in seconds to show the message.</param>
	public static void DisplayMessage(string message, float displayTime = 10.0f)
	{
		Controller?.MessageFieldText?.DisplayMessage(message, displayTime);
	}
	#endregion

	#region Interaction checks
	/// <summary>
	/// Checks whether an object at the specified position should be affected by a press on the <seealso cref="InteractionKey"/>.
	/// </summary>
	/// <param name="interactablePosition">Position to be checked.</param>
	/// <returns><c>true</c> if there should be an interaction, <c>false</c> otherwise.</returns>
	public bool CheckForInteraction(Vector3 interactablePosition, float interactionDistance, bool useDefaultDistance, bool continousInteraction = false)
	{
		// Make sure that a player reference is set
		if (Player == null || PlayerHasDied)
			return false;

		// Check whether the interaction key was pressed
		if (Input.GetKeyDown(InteractionKey) || (continousInteraction && Input.GetKey(InteractionKey)))
		{
			// Check whether the player is within interaction distance
			Vector3 playerToObject = interactablePosition - Player.transform.position;
			if (playerToObject.magnitude < (useDefaultDistance ? InteractionDistance : interactionDistance))
			{
				// Check whether the player is facing the object
				Vector3 forward = Player.transform.TransformDirection(Vector3.forward);
				forward.y = 0;			// Only consider the horizontal plane
				playerToObject.y = 0;
				if (Mathf.Acos(Vector3.Dot(playerToObject.normalized, forward.normalized)) * Mathf.Rad2Deg < InteractionArcDegrees / 2.0f)
					return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Checks whether the player should interact with an object (without pressing a key).
	/// </summary>
	/// <param name="interactablePosition">Position to be checked.</param>
	/// <returns><c>true</c> if there should be an interaction, <c>false</c> otherwise.</returns>
	public bool CheckForAutoInteraction(Vector3 interactablePosition, float interactionDistance, bool useDefaultDistance)
	{
		// Make sure that a player reference is set
		if (Player == null || PlayerHasDied)
			return false;

		// Check whether the player is within interaction distance
		Vector3 v = interactablePosition - Player.transform.position;
		if (v.magnitude < (useDefaultDistance ? AutoInteractionDistance : interactionDistance))
			return true;

		return false;
	}
	#endregion

	#region Keys (CheckKey)
	/// <summary>
	/// Checks whether the player has acquired the key.
	/// </summary>
	/// <param name="key">The key to check.</param>
	/// <returns><c>true</c> if the key was acquired, <c>false</c> otherwise.</returns>
	public bool CheckKey(Key.KeyType key)
	{
		try
		{
			return HasKey[(int)key];
		}
		catch(System.Exception e)
		{
			Debug.LogException(e);
		}

		return false;
	}
	#endregion

	#region Ammo Get/Set/Add
	public void SetAmmo(WeaponSpecification.WeaponAmmoType type, uint value)
	{
		if (type == WeaponSpecification.WeaponAmmoType.Infinite)
			return;

		try
		{
			Ammo[(int)type] = value;
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
		}
	}

	public void AddAmmo(WeaponSpecification.WeaponAmmoType type, uint delta)
	{
		AddAmmo(type, (int)delta);
	}

	public void AddAmmo(WeaponSpecification.WeaponAmmoType type, int delta)
	{
		if (type == WeaponSpecification.WeaponAmmoType.Infinite)
			return;

		try
		{
			if(delta >= 0)
				Ammo[(int)type] += (uint)delta;
			else
				Ammo[(int)type] -= (uint)(-delta);

			// Limit ammo
			Ammo[(int)type] = Math.Min(Ammo[(int)type], MaxAmmo[(int)type]);
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
		}
	}

	public uint GetAmmo(WeaponSpecification.WeaponAmmoType type)
	{
		// Treat max value as infinite (for all practical purposes it will hold true)
		if (type == WeaponSpecification.WeaponAmmoType.Infinite)
			return uint.MaxValue;

		try
		{
			return Ammo[(int)type];
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
		}

		return 0;
	}
	#endregion

	#region Weapons (CheckFireWeapon, SetHasWeapon)
	/// <summary>
	/// Checks whether the player has pressed the <seealso cref="FireKey"/>.
	/// </summary>
	/// <param name="weapon">Weapon to fire.</param>
	/// <returns><c>true</c> if the player has pressed the <seealso cref="FireKey"/> and weapon state is ok, <c>false</c> otherwise.</returns>
	public bool CheckFireWeaponRequested(WeaponSpecification weapon)
	{
		if (weapon.State == WeaponSpecification.WeaponState.Idle && ((weapon.AutomaticFire && Input.GetKey(FireKey)) || Input.GetKeyDown(FireKey))
			&& !PlayerHasDied)
			return true;

		return false;
	}

	/// <summary>
	/// Checks whether the player has pressed the <seealso cref="ReloadKey"/>.
	/// </summary>
	/// <param name="weapon">Weapon to reload.</param>
	/// <returns><c>true</c> if the player has pressed the <seealso cref="ReloadKey"/> and weapon state is ok, <c>false</c> otherwise.</returns>
	public bool CheckReloadWeaponRequested(WeaponSpecification weapon)
	{
		if (weapon.State == WeaponSpecification.WeaponState.Idle && (Input.GetKeyDown(ReloadKey) || (weapon.AutoReload && weapon.AmmoInClip == 0))
			&& !PlayerHasDied)
			return true;

		return false;
	}

	/// <summary>
	/// Checks whether enough ammo is left for reloading the weapon.
	/// </summary>
	/// <param name="weapon">Weapon to reload.</param>
	/// <returns><c>true</c> if the weapon has enough ammo available, <c>false</c> otherwise.</returns>
	public bool CheckCanReloadWeapon(WeaponSpecification weapon)
	{
		if (weapon.EnableReloading)
		{
			if (GetAmmo(weapon.AmmoType) > 0)	// No need for a full clip in order to reload
				return true;
		}

		return false;
	}

	/// <summary>
	/// Checks whether the player has enough ammo ready in the weapon to fire it.
	/// </summary>
	/// <param name="weapon">Weapon to fire.</param>
	/// <returns><c>true</c> if the weapon has enough ammo available, <c>false</c> otherwise.</returns>
	public bool CheckCanFireWeapon(WeaponSpecification weapon)
	{
		try
		{
			if (weapon.EnableReloading)
			{
				if (weapon.AmmoInClip >= weapon.AmmoCost)
					return true;
				else
					Debug.Log("Not enough ammo in clip, you need to reload your weapon!");
			}
			else if (GetAmmo(weapon.AmmoType) >= weapon.AmmoCost)
			{
				return true;
			}
			else
			{
				Debug.Log("Not enough ammo!");
			}
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
		}

		return false;
	}

	public void SetHasWeapon(WeaponSpecification weapon, bool hasWeapon)
	{
		for(int i = 0; i < Weapons.Length && i < HasWeapon.Length; i++)
		{
			if (Weapons[i] == weapon)
				HasWeapon[i] = hasWeapon;
		}
	}

	public bool GetHasWeapon(WeaponSpecification weapon)
	{
		for (int i = 0; i < Weapons.Length && i < HasWeapon.Length; i++)
		{
			if (Weapons[i] == weapon)
				return HasWeapon[i];
		}

		return false;
	}

	private void CheckSelectWeapon()
	{
		if (!GetHasWeapon(CurrentWeapon))
			SetActiveWeapon(null);

		for(int i = 0; i < Weapons.Length && i < WeaponKeys.Length; i++)
		{
			if (Input.GetKeyDown(WeaponKeys[i]))
				SetActiveWeapon(Weapons[i]);
		}
	}

	public void SetActiveWeapon(WeaponSpecification weapon)
	{
		if (weapon == null || PlayerHasDied)
			return;

		if (GetHasWeapon(weapon))
		{
			CurrentWeapon = weapon;
			weapon.gameObject.SetActive(true);
			weapon.VisualRepresentation?.SetActive(true);
			Debug.Log("Active weapon set to " + weapon.Name);
		}
	}
	#endregion

	public void Die()
	{
		if (Player == null || !PlayerCanDie)
			return;

		PlayerHasDied = true;
		Player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
		CurrentWeapon = null;

		StartCoroutine(AnimateDie());
	}

	private IEnumerator AnimateDie()
	{
		const float deathAnimationSpeed = 1.0f;
		Vector3 currentScale = Player.transform.localScale;
		Vector3 targetScale = new Vector3(currentScale.x, currentScale.y / 100.0f, currentScale.z);
		Quaternion currentRotation = Player.transform.rotation;
		Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, 80.0f));

		var waiter = new WaitForEndOfFrame();

		for (float lambda = 0; lambda < 1; lambda += deathAnimationSpeed * Time.deltaTime)
		{
			Player.transform.localScale = lambda * targetScale + (1.0f - lambda) * currentScale;
			Player.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, lambda);
			yield return waiter;
		}
		Player.transform.localScale = targetScale;
		Player.transform.rotation = targetRotation;

		GameOverPanel?.Show();
	}
}
