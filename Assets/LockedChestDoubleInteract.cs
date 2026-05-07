using System.Collections;
using UnityEngine;

public class LockedChestKeyActivator : MonoBehaviour, IInteractable
{
	[Header("Requirement")]
	public int coinsRequired = 50;

	[Header("Chest Motion")]
	public Transform lidHinge;
	public float openAngle = 90f;
	public float openDuration = 1f;

	[Header("Key")]
	public GameObject keyToActivate;
	public float keyFloatHeight = 1.5f;
	public float keyFloatDuration = 1f;

	[Header("Audio")]
	public AudioSource audioSource;
	public AudioClip lockedSound;
	public AudioClip openSound;

	private bool isOpen;
	private bool isOpening;

	private Quaternion closedRotation;
	private Quaternion openRotation;

	private void Start()
	{
		if (keyToActivate != null)
		{
			keyToActivate.SetActive(false);
		}

		if (lidHinge != null)
		{
			closedRotation = lidHinge.localRotation;
			openRotation = closedRotation * Quaternion.Euler(openAngle, 0f, 0f);
		}
	}

	public void Interact()
	{
		if (isOpen || isOpening) return;

		if (GameManager.Instance == null)
		{
			Debug.LogWarning("No GameManager found.");
			return;
		}

		if (GameManager.Instance.coinsCollected < coinsRequired)
		{
			Debug.Log("Not enough coins. Need " + coinsRequired + " coins.");
			PlaySound(lockedSound);
			return;
		}

		bool spentCoins = GameManager.Instance.SpendCoins(coinsRequired);

		if (!spentCoins)
		{
			PlaySound(lockedSound);
			return;
		}

		StartCoroutine(OpenChestAndRevealKey());
	}

	private IEnumerator OpenChestAndRevealKey()
	{
		isOpening = true;

		PlaySound(openSound);

		float timer = 0f;

		while (timer < openDuration)
		{
			timer += Time.deltaTime;
			float t = timer / openDuration;

			if (lidHinge != null)
			{
				lidHinge.localRotation = Quaternion.Slerp(closedRotation, openRotation, t);
			}

			yield return null;
		}

		if (lidHinge != null)
		{
			lidHinge.localRotation = openRotation;
		}

		if (keyToActivate != null)
		{
			keyToActivate.SetActive(true);
			StartCoroutine(FloatKeyUp(keyToActivate));
		}

		isOpen = true;
		isOpening = false;
	}

	private IEnumerator FloatKeyUp(GameObject key)
	{
		Vector3 startPos = key.transform.position;
		Vector3 endPos = startPos + Vector3.up * keyFloatHeight;

		float timer = 0f;

		while (timer < keyFloatDuration)
		{
			timer += Time.deltaTime;
			float t = timer / keyFloatDuration;

			key.transform.position = Vector3.Lerp(startPos, endPos, t);

			yield return null;
		}

		key.transform.position = endPos;
	}

	private void PlaySound(AudioClip clip)
	{
		if (audioSource != null && clip != null)
		{
			audioSource.PlayOneShot(clip);
		}
	}
}