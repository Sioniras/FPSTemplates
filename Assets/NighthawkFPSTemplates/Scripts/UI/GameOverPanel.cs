using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
	public Image PanelBackground;
	public Text PanelText;
	public float FadeSpeed = 1f;

	private float bgAlpha = 1f;
	private float txtAlpha = 1f;

	public void Show()
	{
		bgAlpha = PanelBackground.color.a;
		txtAlpha = PanelText.color.a;
		PanelBackground.color = new Color(PanelBackground.color.r, PanelBackground.color.g, PanelBackground.color.b, 0);
		PanelText.color = new Color(PanelText.color.r, PanelText.color.g, PanelText.color.b, 0);
		gameObject.SetActive(true);
		StartCoroutine(FadeIn());
	}

	private IEnumerator FadeIn()
	{
		var waiter = new WaitForEndOfFrame();

		while (PanelBackground.color.a < bgAlpha || PanelText.color.a < txtAlpha)
		{
			float currentBgAlpha = Mathf.Min(bgAlpha, PanelBackground.color.a + FadeSpeed * Time.deltaTime);
			float currentTxtAlpha = Mathf.Min(txtAlpha, PanelText.color.a + FadeSpeed * Time.deltaTime);

			PanelBackground.color = new Color(PanelBackground.color.r, PanelBackground.color.g, PanelBackground.color.b, currentBgAlpha);
			PanelText.color = new Color(PanelText.color.r, PanelText.color.g, PanelText.color.b, currentTxtAlpha);

			yield return waiter;
		}

		Debug.Log("Game Over Panel shown");
	}
}
