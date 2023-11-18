using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Saucer : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private int score = 250;
	[SerializeField] private float blinkDuration = .2f;
	[Header("Movement")]
	[SerializeField] float moveSpeed = 5f;
	[SerializeField] float changeDirectionTime = 2f;
	[Header("Sound")]
	[SerializeField] private float playSoundEvery = 6f;
	[Header("References")]
	[SerializeField] private Transform shootRotate;
	[SerializeField] private HealthController healthController;
	[SerializeField] private PowerupDrop powerupDrop;
	[SerializeField] private ParticleSystem explosion;
	[SerializeField] private GameObject spriteFill;

	private float blinkTimer = 0;
	private bool isBlinking = false;
	private float soundTimer = 0f;
	private PlayerController player;
	private Vector3 moveDirection;
	private float changeDirTimer;

	private void Awake()
	{
		player = FindObjectOfType<PlayerController>();

		ChangeDirection();
		changeDirTimer = changeDirectionTime;
		soundTimer = playSoundEvery;
	}

	private void Update()
	{
		// rotate towards player
		shootRotate.LookAt(player.transform);

		// move enemy
		Vector3 moveVec = moveDirection * moveSpeed * Time.deltaTime;
		moveVec.y = 0f; // avoid moving in Y axis
		transform.Translate(moveVec);

		// change direction
		changeDirTimer -= Time.deltaTime;
		if (changeDirTimer <= 0f)
		{
			ChangeDirection();
			changeDirTimer = changeDirectionTime;
		}

		// sound and vfx
		soundTimer += Time.fixedDeltaTime;
		if (soundTimer > playSoundEvery)
		{
			AudioManager.PlaySound("snd_Menacing", 3);
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
		if (collision.gameObject.CompareTag("Enemy"))
			AudioManager.PlaySound("snd_Explosion1", 1);

		if (collision.gameObject.CompareTag("Bullet"))
		{
			spriteFill.SetActive(true);
			isBlinking = true;

			healthController.TakeDamage(1, DestroySaucer);
			if (healthController.CurrentHealth >= 1)
				AudioManager.PlaySound("snd_Hit", 1);

			Destroy(collision.gameObject);
		}
	}

	private void ChangeDirection()
	{
		// change direction to a random one
		Vector2 randomDirection = Random.insideUnitCircle.normalized;
		moveDirection = new Vector3(randomDirection.x, 0f, randomDirection.y);
	}

	private void DestroySaucer()
	{
		GameManager.NumberOfEnemies--;
		GameManager.IncreaseScore(score);

		Instantiate(explosion, transform.position, explosion.transform.rotation);
		powerupDrop.TrySpawnPowerup();
		Destroy(gameObject);

		AudioManager.PlaySound("snd_Explosion3", 2);
	}
}
