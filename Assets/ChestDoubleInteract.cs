using System.Collections;
using UnityEngine;

public class ChestDoubleInteract : MonoBehaviour, IInteractable
{
	public enum RewardType { Coin, Gem }

	[Header("Chest Motion")]
	public Transform lidHinge;           
	public float openAngle = 90f;
	public float openDuration = 4f;

	[Header("Reward Display (NOT a Pickup)")]
	public Transform rewardDisplay;        
	public Vector3 rewardFloatOffset = new Vector3(0, 1f, 0);
	public float floatDuration = 4f;

	[Header("Reward Given On 2nd Press")]
	public RewardType rewardType = RewardType.Gem;
	public int rewardAmount = 1;

	[Header("Sounds")]
	public AudioSource openSound;        
	public AudioSource collectSound;       

	bool opened = false;
	bool readyToCollect = false;
	bool collected = false;

	Vector3 rewardStartPos;
	Vector3 rewardEndPos;

	public void Interact()
	{
		if (collected) return;

		
		if (!opened)
		{
			opened = true;
			StartCoroutine(OpenSequence());
			return;
		}

		
		if (opened && readyToCollect && !collected)
		{
			collected = true;

			if (rewardType == RewardType.Coin) GameManager.Instance.AddCoin(rewardAmount);
			if (rewardType == RewardType.Gem) GameManager.Instance.AddGem(rewardAmount);

			if (collectSound != null) collectSound.Play();

			if (rewardDisplay != null) Destroy(rewardDisplay.gameObject);

		
			var col = GetComponent<Collider>();
			if (col != null) col.enabled = false;
		}
	}

	IEnumerator OpenSequence()
	{
		
		if (rewardDisplay != null)
		{
			rewardDisplay.gameObject.SetActive(true);
			rewardStartPos = rewardDisplay.position;
			rewardEndPos = rewardStartPos + rewardFloatOffset;
		}

		if (openSound != null) openSound.Play();

		float t = 0f;
		Quaternion startRot = lidHinge.localRotation;
		Quaternion endRot = Quaternion.Euler(openAngle, 0f, 0f);

		
		while (t < 1f)
		{
			t += Time.deltaTime / openDuration;
			float eased = Mathf.SmoothStep(0f, 1f, t);

			if (lidHinge != null)
				lidHinge.localRotation = Quaternion.Slerp(startRot, endRot, eased);

			if (rewardDisplay != null)
				rewardDisplay.position = Vector3.Lerp(rewardStartPos, rewardEndPos, eased);

			yield return null;
		}

		
		readyToCollect = true;
	}
}