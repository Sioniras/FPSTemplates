using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplay : MonoBehaviour
{
	public Text AmmoText;

	private uint ammo;
	private uint ammoInClip;
	private WeaponSpecification currentWeapon;

	// Update is called once per frame
	void Update()
	{
		if (GameController.Controller == null || GameController.Controller.CurrentWeapon == null)
			return;

		if(currentWeapon != GameController.Controller.CurrentWeapon
			|| ammo != GameController.Controller.GetAmmo(GameController.Controller.CurrentWeapon.AmmoType)
			|| (GameController.Controller.CurrentWeapon.EnableReloading && ammoInClip != GameController.Controller.CurrentWeapon.AmmoInClip))
		{
			currentWeapon = GameController.Controller.CurrentWeapon;
			ammo = GameController.Controller.GetAmmo(GameController.Controller.CurrentWeapon.AmmoType);
			ammoInClip = GameController.Controller.CurrentWeapon.AmmoInClip;

			AmmoText.text = (currentWeapon.EnableReloading ? (ammoInClip + " / ") : "") + ammo;
		}
	}
}
