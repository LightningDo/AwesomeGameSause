using UnityEngine;
using TMPro;

public class HUDCounts : MonoBehaviour
{
	public TMP_Text coinsText;
	public TMP_Text gemsText;

	void Update()
	{
		
		coinsText.text = "Coins: " + GameManager.Instance.coinsCollected;
		gemsText.text = "Gems: " + GameManager.Instance.gemsCollected;
	}
}