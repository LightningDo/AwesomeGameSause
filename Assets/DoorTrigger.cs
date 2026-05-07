using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
	public DoorAuto door;

	void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		door.SetOpen(true);
	}

	void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		door.SetOpen(false);
	}
}