using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody))]
public class PlayerDash : MonoBehaviour
{
	[Header("Dash")]
	public KeyCode dashKey = KeyCode.LeftShift;
	public float dashDistance = 6f;
	public float dashDuration = 0.15f;
	public float dashCooldown = 1.0f;

	[Header("UI (Cooldown Fill)")]
	// fillAmount: 1 = cooling down, 0 = ready
	public Image dashCooldownFill;

	[Header("Camera (Optional)")]
	public Camera playerCamera;
	public float fovKick = 12f;

	[Header("Warp Effect (Global Volume)")]
	public Volume globalVolume; 
	[Range(0f, 1f)] public float chromaStrength = 0.6f;
	[Range(0f, 1f)] public float lensStrength = 0.35f;

	[Header("Audio (Optional)")]
	public AudioSource dashAudioSource; 
	public AudioClip dashClip;         
	[Range(0f, 1f)] public float dashVolume = 1f;

	private Rigidbody rb;
	private bool isDashing = false;
	private float cooldownTimer = 0f;

	private float baseFov;

	// Volume overrides
	private ChromaticAberration chroma;
	private LensDistortion lens;

	void Start()
	{
		rb = GetComponent<Rigidbody>();

		if (playerCamera == null) playerCamera = Camera.main;
		if (playerCamera != null) baseFov = playerCamera.fieldOfView;

		if (globalVolume == null) globalVolume = FindObjectOfType<Volume>();

		if (globalVolume != null && globalVolume.profile != null)
		{
			globalVolume.profile.TryGet(out chroma);
			globalVolume.profile.TryGet(out lens);
		}

		
		if (dashAudioSource == null)
			dashAudioSource = GetComponent<AudioSource>();

		SetWarp(0f);
	}

	void Update()
	{
		// Tick cooldown
		if (cooldownTimer > 0f)
			cooldownTimer -= Time.deltaTime;

		// Update UI
		if (dashCooldownFill != null)
		{
			float fill = Mathf.Clamp01(cooldownTimer / dashCooldown);
			dashCooldownFill.fillAmount = fill;
		}

		// Dash input
		if (Input.GetKeyDown(dashKey) && !isDashing && cooldownTimer <= 0f)
		{
			StartCoroutine(DashRoutine());
		}
	}

	private IEnumerator DashRoutine()
	{
		isDashing = true;
		cooldownTimer = dashCooldown;

		// Play dash sound
		PlayDashSound();

		
		if (playerCamera != null)
			playerCamera.fieldOfView = baseFov + fovKick;

		// Warp ON
		SetWarp(1f);

		
		Vector3 savedVel = rb.linearVelocity;
		rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);

		Vector3 dashDir = transform.forward;

		float t = 0f;
		while (t < dashDuration)
		{
			float step = (dashDistance / dashDuration) * Time.deltaTime;
			rb.MovePosition(rb.position + dashDir * step);

			t += Time.deltaTime;
			yield return null;
		}

		
		rb.linearVelocity = savedVel;

		// Warp OFF
		SetWarp(0f);

		if (playerCamera != null)
			playerCamera.fieldOfView = baseFov;

		isDashing = false;
	}

	private void PlayDashSound()
	{
		if (dashAudioSource == null) return;

	
		if (dashClip != null)
			dashAudioSource.PlayOneShot(dashClip, dashVolume);
		else if (dashAudioSource.clip != null)
			dashAudioSource.PlayOneShot(dashAudioSource.clip, dashVolume);
	}

	private void SetWarp(float strength01)
	{
		if (chroma != null)
		{
			chroma.intensity.overrideState = true;
			chroma.intensity.value = Mathf.Lerp(0f, chromaStrength, strength01);
		}

		if (lens != null)
		{
			lens.intensity.overrideState = true;
			lens.intensity.value = Mathf.Lerp(0f, -lensStrength, strength01);
		}
	}
}