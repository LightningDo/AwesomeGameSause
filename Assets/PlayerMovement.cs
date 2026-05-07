using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	Rigidbody rb;

	[Header("Movement")]
	public float speed = 6f;

	[Header("Jump")]
	public float jumpForce1 = 5f;
	public float jumpForce2 = 5f;
	public int maxJumps = 2;

	[Header("Ground Check")]
	public Transform groundCheck;
	public float groundCheckRadius = 0.25f;
	public LayerMask groundMask;

	[Header("Jump Audio")]
	public AudioSource jumpAudioSource;
	public AudioClip jump1Clip;   // first jump
	public AudioClip jump2Clip;   // double jump
	[Range(0f, 1f)]
	public float jumpVolume = 1f;

	int jumpsUsed = 0;
	bool isGrounded = false;

	void Start()
	{
		rb = GetComponent<Rigidbody>();

		// Auto grab AudioSource if you forgot to assign one
		if (jumpAudioSource == null)
			jumpAudioSource = GetComponent<AudioSource>();
	}

	void FixedUpdate()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		Vector3 move = (transform.right * h + transform.forward * v) * speed;

		rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
	}

	void Update()
	{
		// Check if player is touching the ground
		isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

		if (isGrounded)
		{
			jumpsUsed = 0;
		}

		if (Input.GetButtonDown("Jump") && jumpsUsed < maxJumps)
		{
			float force = (jumpsUsed == 0) ? jumpForce1 : jumpForce2;

			rb.linearVelocity = new Vector3(rb.linearVelocity.x, force, rb.linearVelocity.z);

			PlayJumpSound(jumpsUsed);

			jumpsUsed++;
		}
	}

	void PlayJumpSound(int jumpIndex)
	{
		if (jumpAudioSource == null) return;

		if (jumpIndex == 0 && jump1Clip != null)
		{
			jumpAudioSource.PlayOneShot(jump1Clip, jumpVolume);
		}
		else if (jumpIndex == 1 && jump2Clip != null)
		{
			jumpAudioSource.PlayOneShot(jump2Clip, jumpVolume);
		}
	}

	void OnDrawGizmosSelected()
	{
		if (groundCheck == null) return;

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
	}
}