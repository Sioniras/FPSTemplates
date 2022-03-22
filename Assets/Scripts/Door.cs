using UnityEngine;

public class Door : MonoBehaviour
{
	public enum DoorState
	{
		Open,       // Open, not moving
		Opening,    // Was closed, now moving towards open state
		Closed,     // Closed, not moving
		Closing,    // Was open, now moving towards closed state
	}

	public enum DoorMovementType
	{
		AbsoluteGlobalSpace,	// Moves transform.position to TargetPosition
		RelativeGlobalSpace,	// Moves transform.position to StartPosition + TargetPosition
		LocalSpace,				// Moves relative to a parent object, i.e. if parent object is rotated, then the motion is rotated similarly
	}

	#region Public fields that can be set in the editor
	[Header("Movement")]
	/// <summary>
	/// Determines how the target position and rotation are used.
	/// </summary>
	public DoorMovementType MovementType = DoorMovementType.LocalSpace;

	/// <summary>
	/// The target position of the door, according to the door movement type.
	/// </summary>
	public Vector3 TargetPosition;

	/// <summary>
	/// The change in Euler angles when the door is open.
	/// </summary>
	public Vector3 DeltaRotation;

	/// <summary>
	/// Movement speed of the door when opening or closing.
	/// </summary>
	public float Speed = 1.0f;

	/// <summary>
	/// Whether the door should close automatically after a while.
	/// </summary>
	public bool CloseAutomatically = false;

	/// <summary>
	/// Time in seconds that the door should remain open before closing automatically.
	/// </summary>
	public float AutoCloseDelay = 15.0f;

	[Header("Interactions")]
	/// <summary>
	/// Whether the player should be able to interact with the door using the <seealso cref="GameController.InteractionKey"/>.
	/// </summary>
	public bool UseInteractionKey = true;

	/// <summary>
	/// Whether the player should be able to open the door by getting close to it.
	/// </summary>
	public bool UseAutoOpen = false;

	[Header("Key")]
	/// <summary>
	/// If set to <c>true</c>, the door can only be opened when the player has the key.
	/// </summary>
	public bool IsLocked = false;

	/// <summary>
	/// If the door is locked, this key is needed in order to interact with the door.
	/// </summary>
	public Key.KeyType DoorKey = Key.KeyType.Key1;

	[Header("Triggers")]
	/// <summary>
	/// Reference to trigger that can interact with the door.
	/// </summary>
	public Trigger DoorTrigger;

	/// <summary>
	/// Triggers can only open the door if <seealso cref="TriggerCanOpen"/> is true.
	/// </summary>
	public bool TriggerCanOpen = true;

	/// <summary>
	/// Triggers can only close the door if <seealso cref="TriggerCanClose"/> is true.
	/// </summary>
	public bool TriggerCanClose = true;
	#endregion

	// Properties
	public DoorState State { get; private set; } = DoorState.Closed;

	// Private fields
	private Vector3 interactionPosition;
	private Vector3 startPosition;
	private Quaternion startRotation;
	private Quaternion targetRotation;
	private float openFraction;     // Linear interpolation fraction
	private float autoCloseTimer;

	// Start is called before the first frame update
	void Start()
	{
		// Get the absolute starting position to use for interaction checks
		interactionPosition = transform.position;

		// Get the start position and parameters in the correct space
		if(MovementType == DoorMovementType.LocalSpace)
		{
			startPosition = transform.localPosition;
			startRotation = transform.localRotation;
		}
		else
		{
			startPosition = transform.position;
			startRotation = transform.rotation;
		}

		// Modify target position if delta is used
		if (MovementType == DoorMovementType.RelativeGlobalSpace || MovementType == DoorMovementType.LocalSpace)
			TargetPosition += startPosition;

		// Get the target rotation
		targetRotation = startRotation * Quaternion.Euler(DeltaRotation);

		// Register trigger
		if(DoorTrigger != null)
			DoorTrigger.TriggerFired += On_TriggerFired;
	}

	// Triggers can open/close the door
	public void On_TriggerFired(Trigger trigger)
	{
		if (State == DoorState.Open && TriggerCanClose)
			State = DoorState.Closing;
		else if (State == DoorState.Closed && TriggerCanOpen)
			State = DoorState.Opening;
	}

	// Update is called once per frame
	void Update()
	{
		switch (State)
		{
			case DoorState.Opening:
				// Update fraction and state
				openFraction += Speed * Time.deltaTime;
				if (openFraction > 1.0f)
				{
					openFraction = 1.0f;
					State = DoorState.Open;
					autoCloseTimer = Time.time;
				}
				MoveDoor();
				break;
			case DoorState.Closing:
				// Update fraction and state
				openFraction -= Speed * Time.deltaTime;
				if (openFraction < 0)
				{
					openFraction = 0;
					State = DoorState.Closed;
				}
				MoveDoor();
				break;
			default:
				// First check whether the door is unlocked or whether the player has the key
				if ((!IsLocked || (GameController.Controller?.CheckKey(DoorKey) ?? false)))
				{
					// If the door is open or closed, check for interaction keyboard press or auto open and change state if needed
					if (UseInteractionKey && (GameController.Controller?.CheckForInteraction(interactionPosition) ?? false))
						State = (State == DoorState.Open ? DoorState.Closing : DoorState.Opening);
					else if (UseAutoOpen && State == DoorState.Closed && (GameController.Controller?.CheckForAutoInteraction(interactionPosition) ?? false))
						State = DoorState.Opening;
				}

				// Check for auto-close
				if(CloseAutomatically && State == DoorState.Open && (Time.time - autoCloseTimer > AutoCloseDelay))
				{
					if (GameController.Controller?.CheckForAutoInteraction(interactionPosition) ?? false)
						autoCloseTimer += 1.0f;
					else
						State = DoorState.Closing;
				}
				break;
		}	
	}

	/// <summary>
	/// Changes the position and rotation of the door according to how open/closed it should be.
	/// </summary>
	private void MoveDoor()
	{
		// Use local space if specified, otherwise global coordinates
		// Position uses linear interpolation, while rotation part uses spherical linear interpolation
		if(MovementType == DoorMovementType.LocalSpace)
		{
			transform.localPosition = openFraction * TargetPosition + (1.0f - openFraction) * startPosition;
			transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, openFraction);
		}
		else
		{
			transform.position = openFraction * TargetPosition + (1.0f - openFraction) * startPosition;
			transform.rotation = Quaternion.Slerp(startRotation, targetRotation, openFraction);
		}
	}
}
