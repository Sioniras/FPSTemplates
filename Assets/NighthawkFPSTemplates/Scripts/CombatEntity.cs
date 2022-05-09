using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEntity : MonoBehaviour
{
	/// <summary>
	/// The amount of health points. The <see cref="IsDead"/> flag is set to <c>true</c> when <see cref="Health"/> reaches <c>0</c>.
	/// </summary>
	public float Health = 100;
	
	/// <summary>
	/// Whether the entity has died. Increasing <seealso cref="Health"/> to a positive value is not enough to change <see cref="IsDead"/>.
	/// Use <seealso cref="Revive(float)"/> for that.
	/// </summary>
	public bool IsDead { get; private set; }

	// Start is called before the first frame update
	void Start()
	{
		IsDead = false;
	}

	// Update is called once per frame
	void Update()
	{
		if(Health < 1e-8)
			IsDead = true;
	}

	/// <summary>
	/// Kills the entity and sets <seealso cref="Health"/> to <c>0</c>.
	/// </summary>
	public void Kill()
	{
		Health = 0;
		IsDead = true;
	}

	/// <summary>
	/// Brings the entity back to life with at least <c>1</c> point of <seealso cref="Health"/>.
	/// </summary>
	/// <param name="health"></param>
	public void Revive(float health = 1.0f)
	{
		Health = System.Math.Max(health, 1.0f);
		IsDead = false;
	}

	/// <summary>
	/// Decreases health by the amount specified as <paramref name="damage"/>.
	/// Cannot increase health, use <seealso cref="Heal(float, float)"/> to increase health.
	/// </summary>
	/// <param name="damage">The amount of health lost.</param>
	public void TakeDamage(float damage)
	{
		if (damage < 0)
			return;

		Health -= (damage < Health) ? damage : Health;
	}

	/// <summary>
	/// Heals the entity without exceeded the <paramref name="maxHealth"/> parameter.
	/// Note that <paramref name="healing"/> must be positive - negative values are ignored.
	/// Use <seealso cref="TakeDamage(float)"/> to decrease health.
	/// </summary>
	/// <param name="healing">The amount of health to add.</param>
	/// <param name="maxHealth">The upper bound for the healing.</param>
	public void Heal(float healing, float maxHealth = float.MaxValue)
	{
		float healed = Health + System.Math.Max(healing, 0);
		Health = (healed <= maxHealth) ? healed : maxHealth;
	}
}
