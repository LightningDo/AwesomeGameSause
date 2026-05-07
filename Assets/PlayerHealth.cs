using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
	[Header("Health")]
	public int maxHealth = 100;
	public int currentHealth;
	public bool isDead = false;

	[Header("UI - HP Text")]
	public TextMeshProUGUI hpText;

	[Header("UI - Low Health Overlay")]
	public Image damageOverlay;
	public float maxOverlayAlpha = 0.5f;

	[Header("UI - Hurt Flash")]
	public Image hurtFlashOverlay;
	public float hurtFlashAlpha = 0.35f;
	public float hurtFlashDuration = 0.1f;

	[Header("UI - Death Text")]
	public GameObject deathTextObject;

	[Header("Death Return Settings")]
	public float deathScreenTime = 3f;
	public string titleSceneName = "StartScene";

	[Header("Audio - Hurt Sound")]
	public AudioSource hurtAudioSource;
	public AudioClip hurtClip;
	[Range(0f, 1f)] public float hurtVolume = 1f;

	[Header("Audio - Death Sound")]
	public AudioClip deathClip;
	[Range(0f, 1f)] public float deathVolume = 1f;

	private Coroutine hurtFlashRoutine;

	void Start()
	{
		currentHealth = maxHealth;
		UpdateHPUI();
		UpdateOverlay();

		if (hurtFlashOverlay != null)
		{
			Color c = hurtFlashOverlay.color;
			c.a = 0f;
			hurtFlashOverlay.color = c;
		}

		if (deathTextObject != null)
		{
			deathTextObject.SetActive(false);
		}
	}

	public void TakeDamage(int damageAmount)
	{
		if (isDead) return;

		currentHealth -= damageAmount;

		if (currentHealth < 0)
			currentHealth = 0;

		PlayHurtSound();
		UpdateHPUI();
		UpdateOverlay();

		if (hurtFlashOverlay != null)
		{
			if (hurtFlashRoutine != null)
				StopCoroutine(hurtFlashRoutine);

			hurtFlashRoutine = StartCoroutine(HurtFlash());
		}

		Debug.Log("Player HP: " + currentHealth);

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	void UpdateHPUI()
	{
		if (hpText != null)
		{
			hpText.text = "HP: " + currentHealth;
		}
	}

	void UpdateOverlay()
	{
		if (damageOverlay == null) return;

		float healthPercent = (float)currentHealth / maxHealth;

		Color c = damageOverlay.color;

		if (healthPercent <= 0.5f)
		{
			float t = 1f - (healthPercent / 0.5f);
			float alpha = Mathf.Lerp(0f, maxOverlayAlpha, t);
			c.a = alpha;
		}
		else
		{
			c.a = 0f;
		}

		damageOverlay.color = c;
	}

	IEnumerator HurtFlash()
	{
		Color c = hurtFlashOverlay.color;
		c.a = hurtFlashAlpha;
		hurtFlashOverlay.color = c;

		yield return new WaitForSeconds(hurtFlashDuration);

		c.a = 0f;
		hurtFlashOverlay.color = c;
	}

	void PlayHurtSound()
	{
		if (hurtAudioSource == null) return;

		if (hurtClip != null)
			hurtAudioSource.PlayOneShot(hurtClip, hurtVolume);
		else if (hurtAudioSource.clip != null)
			hurtAudioSource.PlayOneShot(hurtAudioSource.clip, hurtVolume);
	}

	void PlayDeathSound()
	{
		if (hurtAudioSource == null) return;

		if (deathClip != null)
			hurtAudioSource.PlayOneShot(deathClip, deathVolume);
	}

	void Die()
	{
		isDead = true;

		Debug.Log("Player died.");

		PlayDeathSound();

		if (deathTextObject != null)
		{
			deathTextObject.SetActive(true);
		}

		PlayerMovement movement = GetComponent<PlayerMovement>();
		if (movement != null) movement.enabled = false;

		PlayerDash dash = GetComponent<PlayerDash>();
		if (dash != null) dash.enabled = false;

		PlayerProjectileAttack attack = GetComponent<PlayerProjectileAttack>();
		if (attack != null) attack.enabled = false;

		StartCoroutine(ReturnToTitleScreen());
	}

	IEnumerator ReturnToTitleScreen()
	{
		yield return new WaitForSeconds(deathScreenTime);
		SceneManager.LoadScene(titleSceneName);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			TakeDamage(10);
		}
	}
}