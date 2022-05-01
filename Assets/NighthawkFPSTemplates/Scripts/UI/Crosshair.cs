using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
	public float Range = 200;

	public bool RaytraceCrosshair = true;

	public float RaytraceCooldown = 0.1f;

	private Vector3 _targetPosition = Vector3.zero;
	private Vector3 _previousTarget = Vector3.zero;
	private float _traceTimer = 0;

	// Start is called before the first frame update
	void Start()
	{
		if (RaytraceCrosshair)
		{
			GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
			GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if(RaytraceCrosshair && GameController.Controller != null && GameController.Controller.CurrentWeapon != null)
		{
			_traceTimer += Time.deltaTime;
			if(_traceTimer > RaytraceCooldown)
			{
				_previousTarget = _targetPosition;
				_targetPosition = GetAimingPosition(GameController.Controller.CurrentWeapon);
				_traceTimer = 0;
			}

			float lambda = _traceTimer / RaytraceCooldown;
			Vector3 crossHairPosition = _targetPosition * lambda + (1.0f - lambda) * _previousTarget;
			crossHairPosition.z = 0;
			GetComponent<RectTransform>().position = crossHairPosition;
		}
		else
		{
			GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
			GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
			GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
		}
	}

	public Vector3 GetAimingPosition(WeaponSpecification weapon)
	{
		if (GameController.Controller == null || GameController.Controller.Player == null)
			return Vector3.zero;

		Camera playerCamera = GameController.Controller.Player.GetComponentInChildren<Camera>();
		if (playerCamera == null)
			return Vector3.zero;

		var origin = weapon.VisualRepresentation.transform;
		if (weapon.ShotOrigin == WeaponSpecification.FiringOrigin.PlayerPositionAndOrientation)
			origin = GameController.Controller.Player.transform;
		else if (weapon.ShotOrigin == WeaponSpecification.FiringOrigin.CameraPositionAndOrientation)
			origin = playerCamera.gameObject.transform;

		Vector3 start = origin.position;
		Vector3 direction = origin.TransformDirection(Vector3.forward);

		Vector3 targetPosition = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0);

		if(Physics.Raycast(start, direction, out RaycastHit hitInfo, Range))
		{
			targetPosition = playerCamera.WorldToScreenPoint(hitInfo.point);
		}

		return targetPosition;
	}
}
