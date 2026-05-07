using UnityEngine;

public class LeverInteract : MonoBehaviour, IInteractable
{
	public DoorAuto door;               // reuse your DoorAuto from Part 1
	public AudioSource audioSource;
	public Transform leverHandle;       // optional: a child part to rotate
	public float onAngle = -45f;
	public float offAngle = 45f;

	bool isOn;

	public void Interact()
	{
		isOn = !isOn;

		if (door != null)
			door.SetOpen(isOn);

		if (leverHandle != null)
			leverHandle.localRotation = Quaternion.Euler(isOn ? onAngle : offAngle, 0, 0);

		if (audioSource != null)
			audioSource.Play();
	}
}