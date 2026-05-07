using UnityEngine;

public class Projectile : MonoBehaviour
{
	[Header("Projectile Stats")]
	public float speed = 20f;
	public int damage = 25;
	public float lifeTime = 3f;

	void Start()
	{
		Destroy(gameObject, lifeTime);
	}

	void Update()
	{
		transform.position += transform.forward * speed * Time.deltaTime;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
			return;

		EnemyHealth enemy = other.GetComponent<EnemyHealth>();

		if (enemy == null)
		{
			enemy = other.GetComponentInParent<EnemyHealth>();
		}

		if (enemy != null)
		{
			enemy.TakeDamage(damage, transform.forward);
			Destroy(gameObject);
			return;
		}

		Destroy(gameObject);
	}
}