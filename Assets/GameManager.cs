using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public int coinsCollected;
	public int gemsCollected;

	void Awake()
	{
		if (Instance != null) { Destroy(gameObject); return; }
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void AddCoin(int amount = 1) => coinsCollected += amount;
	public void AddGem(int amount = 1) => gemsCollected += amount;

	public bool SpendCoins(int amount)
	{
		if (coinsCollected < amount)
		{
			return false;
		}

		coinsCollected -= amount;
		return true;
	}
}