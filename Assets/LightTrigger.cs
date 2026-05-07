using UnityEngine;

public class LightTrigger : MonoBehaviour
{
	public Light targetLight;
	public AudioSource audioSource;

	void Start()
	{
		if (targetLight != null) targetLight.enabled = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;

		if (targetLight != null) targetLight.enabled = true;
		if (audioSource != null) audioSource.Play();
	}
}