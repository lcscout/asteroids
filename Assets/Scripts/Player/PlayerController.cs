using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
	#region Fields
	[Header("Movement")]
	[SerializeField] private float moveSpeed;
	[SerializeField] private float maxVelocity;
	[SerializeField] private float rotationSpeed;
	[Header("Collision")]
	[SerializeField] private int invencibilityFrames = 10;
	[SerializeField] private float partsImpulse = 20;
	[Header("Powerup")]
	[SerializeField] private int powerupHeal = 1;
	[Header("References")]
	[SerializeField] private HealthController healthController;
	[SerializeField] private Rigidbody rb;
	[SerializeField] private GameObject visual;
	[SerializeField] private ParticleSystem burst;
	[SerializeField] private ParticleSystem explosion;
	[SerializeField] private List<GameObject> bodyParts = new List<GameObject>();
	[Header("Debug")]
	[SerializeField] private bool isBurstPlaying;
	[SerializeField] private bool isBurstStopped;
	[SerializeField] private List<GameObject> instancedParts = new List<GameObject>();
	#endregion

	#region Unity Messages
	private void Awake()
	{
		StopBurst();
	}

	private void FixedUpdate()
	{
		if (!GameManager.IsGameRunning) return;

		// esperando o player continuar, isso é pro player poder evitar o respawn em cima de um inimigo
		if (GameManager.IsWaitingContinue) return;

		// horizontal calculations
		float horizontal = Input.GetAxis("Horizontal");
		float xForce = horizontal * Time.fixedDeltaTime * moveSpeed;

		// vertical calculations
		float vertical = Input.GetAxis("Vertical");
		float zForce = vertical * Time.fixedDeltaTime * moveSpeed;

		// rotate according to mouse
		float zCam = -Camera.main.transform.position.z;
		Vector3 mouseVector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zCam);
		Vector3 mPos = Camera.main.ScreenToWorldPoint(mouseVector);

		Quaternion targetRotation = Quaternion.LookRotation(mPos - transform.position, Vector3.up);
		targetRotation.x = 0f;
		targetRotation.z = 0f;

		float speed = Mathf.Min(rotationSpeed * Time.deltaTime, 1f);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed);

		// go forward
		Vector3 forceVec = (Vector3.right * xForce) + (Vector3.forward * zForce);
		rb.AddForce(forceVec);

		// clamp velocity
		if (rb.velocity.magnitude > maxVelocity)
			rb.velocity = rb.velocity.normalized * maxVelocity;

		// particle
		if (vertical != 0 || horizontal != 0)
			PlayBurst();
		else if (!isBurstStopped)
			StopBurst();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!GameManager.IsGameRunning) return;

		// isso evita multiplos danos em sequencia, alem do player poder evitar o respawn em cima de um inimigo
		if (GameManager.IsWaitingContinue) return;

		if (collision.gameObject.CompareTag("Bullet"))
		{
			Destroy(collision.gameObject);
			PlayerExplosion();
			StopBurst();
			GameManager.WaitContinue();

			healthController.TakeDamage(1, GameManager.Instance.Defeat);
			GameManager.UpdateHealth();

			AudioManager.PlaySound("snd_Explosion2", 0);
		}

		if (collision.gameObject.CompareTag("Enemy"))
		{
			PlayerExplosion();
			StopBurst();
			GameManager.WaitContinue();

			healthController.TakeDamage(1, GameManager.Instance.Defeat);
			GameManager.UpdateHealth();

			AudioManager.PlaySound("snd_Explosion2", 0);
		}
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Powerup"))
		{
			Destroy(other.gameObject);

			healthController.HealOrAdd(powerupHeal);
			GameManager.UpdateHealth();

			AudioManager.PlaySound("snd_Powerup", 4);
		}
	}
	#endregion

	public void HidePlayer()
	{
		visual.SetActive(false);
	}

	public void ShowPlayer()
	{
		transform.position = Vector3.zero;

		foreach (var part in instancedParts)
			Destroy(part);

		instancedParts.Clear();
		visual.SetActive(true);
	}

	private void PlayerExplosion()
	{
		Instantiate(explosion, transform.position, explosion.transform.rotation);

		// instancia partes do player com um força com dir random
		foreach (var part in bodyParts)
		{
			GameObject instancedPart = Instantiate(part, transform.position + part.transform.position, transform.rotation);
			instancedParts.Add(instancedPart);

			Rigidbody rb = instancedPart.GetComponent<Rigidbody>();
			Vector3 randomDir = Random.insideUnitCircle.normalized;
			randomDir.y = 0f;
			rb.AddForce(randomDir * partsImpulse, ForceMode.Impulse);
		}
	}

	private void PlayBurst()
	{
		if (!isBurstPlaying)
		{
			burst.Clear();
			burst.Play();
			isBurstStopped = false;
			isBurstPlaying = true;
		}
	}

	private void StopBurst()
	{
		if (!isBurstStopped)
		{
			burst.Stop();
			isBurstStopped = true;
			isBurstPlaying = false;
		}
	}
}
