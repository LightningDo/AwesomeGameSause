using UnityEngine;

public class PlayerProjectileAttack : MonoBehaviour
{
	[Header("References")]
	public GameObject projectilePrefab;
	public Transform firePoint;
	public Camera playerCamera;

	[Header("Shoot Settings")]
	public float shootCooldown = 0.3f;
	public float maxShootDistance = 100f;

	[Header("Audio")]
	public AudioSource audioSource;
	public AudioClip shootSound;

	private float nextShootTime;

	void Update()
	{
		if (Input.GetMouseButtonDown(0) && Time.time >= nextShootTime)
		{
			Shoot();
			nextShootTime = Time.time + shootCooldown;
		}
	}

	void Shoot()
	{
		if (projectilePrefab == null || firePoint == null || playerCamera == null)
		{
			Debug.LogWarning("Missing projectilePrefab, firePoint, or playerCamera!");
			return;
		}

		Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

		Vector3 targetPoint;

		if (Physics.Raycast(ray, out RaycastHit hit, maxShootDistance))
		{
			targetPoint = hit.point;
		}
		else
		{
			targetPoint = ray.GetPoint(maxShootDistance);
		}

		Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

		GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(shootDirection));

		if (audioSource != null && shootSound != null)
		{
			audioSource.PlayOneShot(shootSound);
		}
	}
}