using UnityEngine;

public class ButtonInteract : MonoBehaviour, IInteractable
{
	public Light targetLight;
	public AudioSource audioSource;

	bool isOn;

	public void Interact()
	{
		isOn = !isOn;

		if (targetLight != null)
			targetLight.enabled = isOn;

		if (audioSource != null)
			audioSource.Play();
	}
}