using UnityEngine;

public class RotateMe : MonoBehaviour
{
	/// <summary>
	/// Euler angles to rotate per second.
	/// </summary>
	public Vector3 AngularVelocity = new Vector3(0, 30, 0);

	/// <summary>
	/// Whether to use <seealso cref="Transform.localRotation"/> instead of <seealso cref="Transform.rotation"/>.
	/// </summary>
	public bool UseLocalSpace = true;

	// Update is called once per frame
	void Update()
	{
		if (UseLocalSpace)
			transform.localRotation *= Quaternion.Euler(AngularVelocity * Time.deltaTime);
		else
			transform.rotation *= Quaternion.Euler(AngularVelocity * Time.deltaTime);
	}
}
