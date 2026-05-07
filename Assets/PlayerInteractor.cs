using UnityEngine;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
	public float interactRange = 3f;
	public KeyCode interactKey = KeyCode.E;

	public GameObject interactUI;   // drag InteractText here

	void Update()
	{
		Ray ray = new Ray(transform.position, transform.forward);

		if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
		{
			var interactable = hit.collider.GetComponentInParent<IInteractable>();

			if (interactable != null)
			{
				if (interactUI != null)
					interactUI.SetActive(true);

				if (Input.GetKeyDown(interactKey))
					interactable.Interact();

				return;
			}
		}

		if (interactUI != null)
			interactUI.SetActive(false);
	}
}