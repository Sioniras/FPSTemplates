using UnityEngine;

public class Trigger : MonoBehaviour
{
	public delegate void TriggerEvent(Trigger trigger);

	#region Public fields that can be set in the editor
	[Header("Key")]
	/// <summary>
	/// If set to <c>true</c>, the door can only be opened when the player has the key.
	/// </summary>
	public bool IsLocked = false;

	/// <summary>
	/// If the door is locked, this key is needed in order to interact with the door.
	/// </summary>
	public Key.KeyType TriggerKey = Key.KeyType.Key1;

	[Header("Interactions")]
	/// <summary>
	/// Whether the player should be able to interact with the trigger using the <seealso cref="GameController.InteractionKey"/>.
	/// </summary>
	public bool UseInteractionKey = true;

	/// <summary>
	/// Whether the player should be able to interact with the trigger by getting close to it.
	/// </summary>
	public bool UseAutoTrigger = false;

	public float AutoTriggerDelay = 5.0f;

	/// <summary>
	/// A custom interaction distance that is used only if <seealso cref="UseDefaultInteractionDistance"/> is set to false.
	/// </summary>
	public float CustomInteractionDistance = 2.5f;

	/// <summary>
	/// Interaction distance is set to <seealso cref="GameController.InteractionDistance"/> if this is true, otherwise its set to <seealso cref="CustomInteractionDistance"/>.
	/// </summary>
	public bool UseDefaultInteractionDistance = true;
	#endregion

	public event TriggerEvent TriggerFired;

	private float autoTriggeredTime = float.NegativeInfinity;

	// Update is called once per frame
	void Update()
	{
		bool hasAutoTriggered = (UseAutoTrigger && (GameController.Controller?.CheckForAutoInteraction(transform.position, CustomInteractionDistance, UseDefaultInteractionDistance) ?? false));

		if (hasAutoTriggered && (Time.time - autoTriggeredTime < AutoTriggerDelay))
			hasAutoTriggered = false;

		// If the door is open or closed, check for interaction and change state if needed
		// Also check whether the door is unlocked or whether the player has the key
		if (((UseInteractionKey && (GameController.Controller?.CheckForInteraction(transform.position, CustomInteractionDistance, UseDefaultInteractionDistance) ?? false)) || hasAutoTriggered)
			&& (!IsLocked || (GameController.Controller?.CheckKey(TriggerKey) ?? false)))
		{
			if(TriggerFired != null)
				TriggerFired(this);

			if (hasAutoTriggered)
				autoTriggeredTime = Time.time;
		}
	}
}
