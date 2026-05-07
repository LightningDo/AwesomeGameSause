using UnityEngine;

public class DoorAuto : MonoBehaviour
{
	public Transform door;
	public Vector3 openOffset = new Vector3(0, 0, 2f);
	public float speed = 3f;
	public AudioSource audioSource;

	Vector3 closedPos;
	Vector3 openPos;
	bool opening;

	void Start()
	{
		if (door == null) door = transform;
		closedPos = door.position;
		openPos = closedPos + openOffset;
	}

	void Update()
	{
		Vector3 target = opening ? openPos : closedPos;
		door.position = Vector3.Lerp(door.position, target, Time.deltaTime * speed);
	}

	public void SetOpen(bool value)
	{
		opening = value;
		if (audioSource != null) audioSource.Play();
	}
}