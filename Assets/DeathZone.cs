using UnityEngine;

public class DeathZone : MonoBehaviour
{
	public int fallDamage = 9999;

	private void OnTriggerEnter(Collider other)
	{
		PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

		if (playerHealth == null)
		{
			playerHealth = other.GetComponentInParent<PlayerHealth>();
		}

		if (playerHealth != null)
		{
			playerHealth.TakeDamage(fallDamage);
		}
	}
}