using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
	[Header("Health")]
	public int maxHealth = 100;
	private int currentHealth;

	[Header("Hit Feedback")]
	public Renderer enemyRenderer;
	public Color hitColor = Color.red;
	public float flashDuration = 0.15f;

	[Header("Knockback")]
	public Rigidbody rb;
	public float knockbackForce = 5f;
	public float knockbackUpwardForce = 2f;

	private Color originalColor;
	private bool isDead = false;

	void Start()
	{
		currentHealth = maxHealth;

		if (enemyRenderer == null)
		{
			enemyRenderer = GetComponentInChildren<Renderer>();
		}

		if (rb == null)
		{
			rb = GetComponent<Rigidbody>();
		}

		if (enemyRenderer != null)
		{
			originalColor = enemyRenderer.material.color;
		}
	}

	public void TakeDamage(int damage, Vector3 hitDirection)
	{
		if (isDead) return;

		currentHealth -= damage;

		if (enemyRenderer != null)
		{
			StartCoroutine(FlashRed());
		}

		ApplyKnockback(hitDirection);

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	IEnumerator FlashRed()
	{
		enemyRenderer.material.color = hitColor;
		yield return new WaitForSeconds(flashDuration);
		enemyRenderer.material.color = originalColor;
	}

	void ApplyKnockback(Vector3 hitDirection)
	{
		if (rb == null) return;

		Vector3 knockback = hitDirection.normalized * knockbackForce;
		knockback.y = knockbackUpwardForce;

		rb.linearVelocity = Vector3.zero;
		rb.AddForce(knockback, ForceMode.Impulse);
	}

	void Die()
	{
		isDead = true;
		Destroy(gameObject);
	}
}