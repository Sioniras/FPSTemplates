using UnityEngine;
using UnityEngine.UI;

public class MessageField : MonoBehaviour
{
	private Text _text;
	private float _displayTime;

	// Start is called before the first frame update
	void Start()
	{
		_text = this.GetComponent<Text>();
	}

	// Update is called once per frame
	void Update()
	{
		_displayTime -= Time.deltaTime;
		if (_displayTime < 0)
			gameObject.SetActive(false);
	}

	public void DisplayMessage(string message, float displayTime = 10.0f)
	{
		_text.text = message;
		_displayTime = displayTime;
		gameObject.SetActive(true);
	}
}
