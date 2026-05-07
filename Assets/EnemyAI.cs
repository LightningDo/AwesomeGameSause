using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	[Header("Target")]
	public Transform player;

	[Header("Notice Box")]
	public Vector3 noticeBoxSize = new Vector3(12f, 4f, 12f);
	public Vector3 noticeBoxOffset = new Vector3(0f, 0f, 0f);

	[Header("Movement")]
	public float moveSpeed = 3f;
	public float stopDistance = 4f;

	[Header("Attack")]
	public int damage = 10;
	public float attackRange = 6f;
	public float attackCooldown = 3f;

	[Header("Charge Attack")]
	public float chargeTime = 2f;
	public float lockPositionDelay = 1.5f; // player position is saved 0.5 sec before dash if chargeTime = 2
	public float dashDuration = 0.25f;
	public float hitRadius = 1.2f;

	[Header("Visuals")]
	public Renderer enemyRenderer;
	public Color normalColor = Color.white;
	public Color chargeColor = Color.yellow;
	public float flashSpeed = 8f;

	[Header("Audio")]
	public AudioSource audioSource;
	public AudioClip chargeClip;
	public AudioClip dashClip;
	[Range(0f, 1f)] public float audioVolume = 1f;

	private float attackTimer = 0f;
	private bool isCharging = false;
	private bool isDashing = false;
	private bool hasHitPlayerThisDash = false;

	void Start()
	{
		if (player == null)
		{
			GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
			if (playerObject != null)
				player = playerObject.transform;
		}

		if (enemyRenderer == null)
			enemyRenderer = GetComponent<Renderer>();

		if (audioSource == null)
			audioSource = GetComponent<AudioSource>();

		if (enemyRenderer != null)
			enemyRenderer.material.color = normalColor;
	}

	void Update()
	{
		if (player == null) return;

		if (attackTimer > 0f)
			attackTimer -= Time.deltaTime;

		if (isCharging || isDashing) return;

		// Only do anything if player is inside the notice box
		if (!PlayerInNoticeBox())
			return;

		float distanceToPlayer = Vector3.Distance(transform.position, player.position);

		// Move toward player until within attack range
		if (distanceToPlayer > attackRange)
		{
			Vector3 direction = (player.position - transform.position).normalized;
			direction.y = 0f;

			transform.position += direction * moveSpeed * Time.deltaTime;

			if (direction != Vector3.zero)
				transform.rotation = Quaternion.LookRotation(direction);
		}
		else if (attackTimer <= 0f)
		{
			StartCoroutine(ChargeAndDashAttack());
		}
	}

	bool PlayerInNoticeBox()
	{
		if (player == null) return false;

		// Convert player position into enemy local space
		Vector3 localPlayerPos = transform.InverseTransformPoint(player.position);

		// Shift by notice box offset
		Vector3 relative = localPlayerPos - noticeBoxOffset;

		return Mathf.Abs(relative.x) <= noticeBoxSize.x * 0.5f &&
			   Mathf.Abs(relative.y) <= noticeBoxSize.y * 0.5f &&
			   Mathf.Abs(relative.z) <= noticeBoxSize.z * 0.5f;
	}

	IEnumerator ChargeAndDashAttack()
	{
		isCharging = true;
		hasHitPlayerThisDash = false;

		// Face player before charging
		FacePlayer();

		
		PlaySound(chargeClip);

		float timer = 0f;
		Vector3 lockedTargetPosition = player.position;

		while (timer < chargeTime)
		{
			timer += Time.deltaTime;

			// If player leaves the notice box during charge, cancel the attack
			if (!PlayerInNoticeBox())
			{
				ResetColor();
				isCharging = false;
				yield break;
			}

			FacePlayer();

			// Flash yellow while charging
			if (enemyRenderer != null)
			{
				float flash = Mathf.Abs(Mathf.Sin(timer * flashSpeed));
				enemyRenderer.material.color = Color.Lerp(normalColor, chargeColor, flash);
			}

			// Save player's position 0.5 seconds before dash
			if (timer >= lockPositionDelay)
			{
				lockedTargetPosition = player.position;
			}

			yield return null;
		}

		ResetColor();
		isCharging = false;

		yield return StartCoroutine(DashToPosition(lockedTargetPosition));

		attackTimer = attackCooldown;
	}

	IEnumerator DashToPosition(Vector3 targetPosition)
	{
		isDashing = true;

		PlaySound(dashClip);

		Vector3 startPosition = transform.position;
		targetPosition.y = startPosition.y;

		Vector3 dashDirection = (targetPosition - startPosition).normalized;
		if (dashDirection != Vector3.zero)
			transform.rotation = Quaternion.LookRotation(dashDirection);

		float timer = 0f;

		while (timer < dashDuration)
		{
			timer += Time.deltaTime;
			float t = timer / dashDuration;

			transform.position = Vector3.Lerp(startPosition, targetPosition, t);

			// Damage player if close enough during dash
			if (!hasHitPlayerThisDash && player != null)
			{
				float distanceToPlayer = Vector3.Distance(transform.position, player.position);
				if (distanceToPlayer <= hitRadius)
				{
					PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
					if (playerHealth != null)
					{
						playerHealth.TakeDamage(damage);
						hasHitPlayerThisDash = true;
					}
				}
			}

			yield return null;
		}

		isDashing = false;
	}

	void FacePlayer()
	{
		if (player == null) return;

		Vector3 direction = player.position - transform.position;
		direction.y = 0f;

		if (direction != Vector3.zero)
			transform.rotation = Quaternion.LookRotation(direction);
	}

	void PlaySound(AudioClip clip)
	{
		if (audioSource == null || clip == null) return;
		audioSource.PlayOneShot(clip, audioVolume);
	}

	void ResetColor()
	{
		if (enemyRenderer != null)
			enemyRenderer.material.color = normalColor;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Matrix4x4 oldMatrix = Gizmos.matrix;

		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		Gizmos.DrawWireCube(noticeBoxOffset, noticeBoxSize);

		Gizmos.matrix = oldMatrix;

		// Attack range ring
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}
}