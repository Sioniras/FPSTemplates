using UnityEngine;

public class Pushable : MonoBehaviour
{
	[Header("Movement")]
	/// <summary>
	/// How fast the object can be pushed, i.e. how heavy it is
	/// </summary>
	public float pushSpeed = 1.0f;

	/// <summary>
	/// Fall speed for "fake/constant velocity" gravity (not using a proper acceleration)
	/// </summary>
	public float fallSpeed = 10.0f;

	[Header("Interactions")]
	/// <summary>
	/// A custom interaction distance that is used only if <seealso cref="UseDefaultInteractionDistance"/> is set to false.
	/// </summary>
	public float CustomInteractionDistance = 2.5f;

	/// <summary>
	/// Interaction distance is set to <seealso cref="GameController.InteractionDistance"/> if this is true, otherwise its set to <seealso cref="CustomInteractionDistance"/>.
	/// </summary>
	public bool UseDefaultInteractionDistance = true;

	private CharacterController controller;

	// Start is called before the first frame update
	void Start()
	{
		controller = GetComponent<CharacterController>();
		if (controller == null)
			controller = this.gameObject.AddComponent<CharacterController>();
	}

	// Update is called once per frame
	void Update()
	{
		if (GameController.Controller?.CheckForInteraction(transform.position, CustomInteractionDistance, UseDefaultInteractionDistance, true) ?? false)
		{
			Vector3 velocity = (transform.position - GameController.Controller.Player.transform.position).normalized * pushSpeed * Time.deltaTime;
			controller.Move(velocity);
		}

		// Fake gravity - note that the fall speed is constant here... Physically wrong, but the player probably will not notice/care anyway
		controller.Move(Vector3.down * fallSpeed * Time.deltaTime);
	}
}
