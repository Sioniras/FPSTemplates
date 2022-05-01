using UnityEngine;

public abstract class WeaponFireSpecification : MonoBehaviour
{
	public virtual void FireWeapon(WeaponSpecification weapon)
	{
		Debug.Log("Firing weapon " + weapon.Name);
	}
}
