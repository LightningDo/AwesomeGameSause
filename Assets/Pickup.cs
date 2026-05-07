using UnityEngine;

public enum PickupType { Coin, Gem }

public class Pickup : MonoBehaviour
{
	public PickupType type;
	public int amount = 1;

	[Header("Sound")]
	public AudioSource audioSource;

	bool collected;

	void Reset()
	{
		
		var col = GetComponent<Collider>();
		if (col != null) col.isTrigger = true;
	}

	void OnTriggerEnter(Collider other)
	{
		if (collected) return;
		if (!other.CompareTag("Player")) return;

		collected = true;

		if (type == PickupType.Coin) GameManager.Instance.AddCoin(amount);
		else if (type == PickupType.Gem) GameManager.Instance.AddGem(amount);

		if (audioSource != null && audioSource.clip != null)
		{
			audioSource.Play();
			// hide immediately so it feels collected
			HideVisuals();
			Destroy(gameObject, audioSource.clip.length);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void HideVisuals()
	{
		// disable visuals so it "disappears" but sound can finish
		foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;
		foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
	}
}