using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private int score = 25;
	[SerializeField] private float blinkDuration = .2f;
	[Header("Movement")]
	[SerializeField] private float spawnImpulse = 20f;
	[SerializeField] private float moveSpeed;
	[SerializeField] private float maxVelocity;
	[Header("Sound")]
	[SerializeField] private float hitSoundCooldown = 2f;
	[Header("References")]
	[SerializeField] private List<GameObject> asteroidsToSpawn;
	[SerializeField] private Rigidbody rb;
	[SerializeField] private HealthController healthController;
	[SerializeField] private ParticleSystem explosionSmall;
	[SerializeField] private ParticleSystem explosion;
	[SerializeField] private GameObject spriteFill;

	private float blinkTimer = 0;
	private bool isBlinking = false;
	private float soundTimer = 0;
	private bool canPlaySound = true;

	private void Awake()
	{
		isBlinking = false;
		blinkTimer = 0;
		spriteFill.SetActive(false);
		soundTimer = 0;
		canPlaySound = true;
	}

	private void Start()
	{
		// adds a impulse force on spawn
		rb.AddForce(transform.forward * spawnImpulse, ForceMode.Impulse);
	}

	private void FixedUpdate()
	{
		// go forward
		rb.AddForce(transform.forward * Time.fixedDeltaTime * moveSpeed);

		// clamp velocity
		if (rb.velocity.magnitude > maxVelocity)
			rb.velocity = rb.velocity.normalized * maxVelocity;

		// sound and vfx
		soundTimer += Time.deltaTime;
		if (soundTimer > hitSoundCooldown)
		{
			canPlaySound = true;
			soundTimer = 0;
		}

		if (isBlinking)
		{
			blinkTimer += Time.deltaTime;
			if (blinkTimer > blinkDuration)
			{
				isBlinking = false;
				blinkTimer = 0;
				spriteFill.SetActive(false);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("BodyPart"))
		{
			Instantiate(explosionSmall, collision.contacts[0].point, explosionSmall.transform.rotation);

			if (canPlaySound)
			{
				AudioManager.PlaySound("snd_Explosion1", 1);
				canPlaySound = false;
			}
		}

		if (collision.gameObject.CompareTag("Bullet"))
		{
			spriteFill.SetActive(true);
			isBlinking = true;

			healthController.TakeDamage(1, DestroyAsteroid);
			if (healthController.CurrentHealth >= 1)
				AudioManager.PlaySound("snd_Hit", 1);

			Destroy(collision.gameObject);
		}
    }

	private void DestroyAsteroid()
	{
		GameManager.NumberOfEnemies--;
		GameManager.IncreaseScore(score);

		Instantiate(explosion, transform.position, explosion.transform.rotation);
		SpawnAsteroids();
		Destroy(gameObject);

		AudioManager.PlaySound("snd_Explosion3", 2);
	}

	private void SpawnAsteroids()
    {
		if (asteroidsToSpawn.Count <= 0) return;

        for (int i = 0; i < asteroidsToSpawn.Count; i++)
        {
			EnemySpawner.Spawn(asteroidsToSpawn[i], transform.position);
        }
    }
}
