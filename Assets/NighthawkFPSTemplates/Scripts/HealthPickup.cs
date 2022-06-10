using UnityEngine;

public class HealthPickup : MonoBehaviour
{
	[Header("Health Pack")]
	/// <summary>
	/// How much health is picked up.
	/// </summary>
	public float HealthAmount = 25.0f;

	/// <summary>
	/// Name showed in pickup-message.
	/// </summary>
	public string Name = "Health Pack";

	[Header("Interactions")]
	/// <summary>
	/// Whether the player should be able to pickup using the <seealso cref="GameController.InteractionKey"/>.
	/// </summary>
	public bool UseInteractionKey = false;

	/// <summary>
	/// Whether the player should be able to pickup by getting close to it.
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
		// Check whether it can be picked up
		if ((UseInteractionKey && (GameController.Controller?.CheckForInteraction(transform.position, CustomInteractionDistance, UseDefaultInteractionDistance) ?? false))
			|| (UseAutoPickup && (GameController.Controller?.CheckForAutoInteraction(transform.position, CustomInteractionDistance, UseDefaultInteractionDistance) ?? false)))
		{
			if (GameController.Controller.PlayerCombatEntity != null)
				GameController.Controller.PlayerCombatEntity.Heal(HealthAmount, GameController.Controller.PlayerMaxHealth);

			GameController.DisplayMessage("Found " + Name);

			if (GameController.Controller.HealthPickupSound != null)
				GameController.Controller.HealthPickupSound.Play();

			// Then remove it
			gameObject.SetActive(false);
		}
	}
}
