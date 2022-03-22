using UnityEngine;

public class Key : MonoBehaviour
{
	/// <summary>
	/// Which type of key to use - set the key names in the GameController instance.
	/// </summary>
	public enum KeyType : byte
	{
		Key1 = 0,
		Key2 = 1,
		Key3 = 2,
		Key4 = 3,
		Key5 = 4,
		Key6 = 5,
		Key7 = 6,
		Key8 = 7,
	}

	[Header("Key Type")]
	/// <summary>
	/// Which type of key to use - set the key names in the GameController instance.
	/// </summary>
	public KeyType Type;

	[Header("Interactions")]
	/// <summary>
	/// Whether the player should be able to pickup the key using the <seealso cref="GameController.InteractionKey"/>.
	/// </summary>
	public bool UseInteractionKey = false;

	/// <summary>
	/// Whether the player should be able to pickup the key by getting close to it.
	/// </summary>
	public bool UseAutoPickup = true;

	// Update is called once per frame
	void Update()
	{
		// Check whether the key can be picked up
		if ((UseInteractionKey && (GameController.Controller?.CheckForInteraction(transform.position) ?? false))
			|| (UseAutoPickup && (GameController.Controller?.CheckForAutoInteraction(transform.position) ?? false)))
		{
			// Register that the player found the key
			try
			{
				GameController.Controller.HasKey[(int)Type] = true;
			}
			catch(System.Exception e)
			{
				Debug.LogException(e);
			}

			// Then remove it
			Destroy(this.gameObject);
		}
	}
}
