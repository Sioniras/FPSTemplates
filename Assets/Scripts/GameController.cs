using UnityEngine;

public class GameController : MonoBehaviour
{
	// Let other scripts reference the controller
	public static GameController Controller { get; private set; }

	[Header("Player")]
	public GameObject Player;

	[Header("Interactable Setup")]
	public KeyCode InteractionKey = KeyCode.E;
	public float InteractionDistance = 2.5f;
	public float AutoInteractionDistance = 2.0f;

	[Header("Keys")]
	public string KeyName1 = "Red Key";
	public string KeyName2 = "Green Key";
	public string KeyName3 = "Blue Key";
	public string KeyName4 = "Yellow Key";
	public string KeyName5 = "Purple Key";
	public string KeyName6 = "Golden Key";
	public string KeyName7 = "Key Code";
	public string KeyName8 = "Access Card";
	public bool[] HasKey = new bool[8];

	// Start is called before the first frame update
	void Start()
	{
		Controller = this;
	}

	// Update is called once per frame
	void Update()
	{
	}

	/// <summary>
	/// Checks whether an object at the specified position should be affected by a press on the <seealso cref="InteractionKey"/>.
	/// </summary>
	/// <param name="interactablePosition">Position to be checked.</param>
	/// <returns><c>true</c> if there should be an interaction, <c>false</c> otherwise.</returns>
	public bool CheckForInteraction(Vector3 interactablePosition)
	{
		// Check whether the interaction key was pressed
		if (Input.GetKey(InteractionKey))
		{
			// Check whether the player is within interaction distance
			Vector3 v = interactablePosition - (Player?.transform.position ?? Vector3.positiveInfinity);
			if (v.magnitude < InteractionDistance)
			{
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
	public bool CheckForAutoInteraction(Vector3 interactablePosition)
	{
		// Check whether the player is within interaction distance
		Vector3 v = interactablePosition - (Player?.transform.position ?? Vector3.positiveInfinity);
		if (v.magnitude < AutoInteractionDistance)
			return true;

		return false;
	}

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
}
