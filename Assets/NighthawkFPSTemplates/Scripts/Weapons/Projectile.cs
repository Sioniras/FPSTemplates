using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float Damage = 10.0f;
	public float Lifetime = 30.0f;

	private float spawnTime;
	private bool hasCollided = false;

	// Start is called before the first frame update
	void Start()
	{
		spawnTime = Time.time;
	}

	// Update is called once per frame
	void Update()
	{
		if (spawnTime + Lifetime < Time.time)
			Destroy(this.gameObject);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (hasCollided)
			return;

		hasCollided = true;

		Debug.Log(collision.gameObject.name);

		CombatEntity target = collision.gameObject.GetComponent<CombatEntity>();
		if (target != null)
		{
			target.TakeDamage(Damage);
		}
	}
}
