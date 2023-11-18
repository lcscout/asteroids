using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	#region Fields
	[Header("Settings")]
	[SerializeField] private float moveSpeed;
	[SerializeField] private float maxDistance = 50;
	[SerializeField] private float lifetime = 3f;
	//[Header("References")]

	private float distanceFromSpawn;
	private Vector3 initialPosition;
	private float lifeTimer;
	#endregion

	#region Properties
	#endregion

	#region Unity Messages
	private void OnEnable()
	{
		// hold and update initial position every time bullet obj is enabled
		initialPosition = transform.position;
	}

	private void Update()
	{
		float zPos = moveSpeed * Time.deltaTime;
		transform.Translate(0, 0, zPos);

		// if bullet goes too far from obj destroy it
		distanceFromSpawn = Vector3.Distance(initialPosition, transform.position);
		if (distanceFromSpawn > maxDistance)
			Destroy(gameObject);

		// or if lifetime runs out destroys it
		lifeTimer += Time.deltaTime;
		if (lifeTimer > lifetime)
		{
			Destroy(gameObject);
			lifeTimer = 0;
        }
	}
	#endregion

	#region Public Methods
	#endregion

	#region Private Methods
	#endregion
}
