using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class ShootController : MonoBehaviour
{
	#region Fields
	[Header("Settings")]
	[SerializeField] private float shootInterval;
	[SerializeField] private bool shootWithKey;
	[SerializeField] private bool canHoldShootKey;
	[SerializeField] private bool randomTimerOnStart;
	[Header("Sound")]
	[SerializeField] private string soundName = "snd_Shoot1";
	[SerializeField] private int track = 0;
	[Header("References")]
	[SerializeField] private Transform spawnPosition;
	[SerializeField] private GameObject bulletPrefab;
	[Header("Debug")]
	[SerializeField] private float shootCooldown = 0;
	[SerializeField] private bool canShoot = false;

	#endregion

	#region Properties
	#endregion

	#region Unity Message
	private void Awake()
	{
		shootCooldown = randomTimerOnStart ? Random.Range(0f, shootInterval) : shootInterval;
		canShoot = false;
	}

	private void Update()
	{
		if (!GameManager.IsGameRunning) return;
		if (GameManager.IsWaitingContinue) return;

		// only count cooldown if you cant shoot
		if (!canShoot)
		{
			shootCooldown -= Time.deltaTime;
			if (shootCooldown <= 0)
			{
				canShoot = true;
				shootCooldown = shootInterval;
			}
		}

		if (shootWithKey)
		{
			// esse Fire1 fica definido lá no Input Manager da unity,
			// igual os quando usa Horizontal e Vertical
			if (canHoldShootKey && Input.GetButton("Fire1"))
				Shoot();
			else if (Input.GetButtonDown("Fire1"))
				Shoot();
		}
		else
			Shoot();
	}
	#endregion

	#region Public Methods
	public void Shoot()
	{
		// do not shoot if you cant
		if (!canShoot) return;

		// find and spawn bullet
		Instantiate(bulletPrefab, spawnPosition.position, spawnPosition.rotation);
		canShoot = false;

		AudioManager.PlaySound(soundName, track);
	}


	#endregion

	#region Private Methods
	#endregion
}
