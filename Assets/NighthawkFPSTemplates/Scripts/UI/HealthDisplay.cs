using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
	public Text HealthText;

	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (GameController.Controller == null)
			return;

		// Only show the health display if the player can die
		HealthText.gameObject.SetActive(GameController.Controller.PlayerCanDie);

		if (GameController.Controller.PlayerCombatEntity == null)
			return;

		// Set health text
		HealthText.text = GameController.Controller.PlayerCombatEntity.Health.ToString("0.");
	}
}
