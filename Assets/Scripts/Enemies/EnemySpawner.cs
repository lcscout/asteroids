using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public static EnemySpawner Instance { get; private set; }

	[System.Serializable]
	public struct Enemy
	{
		public GameObject Prefab;
		public float SpawnChance;
		public bool HasRandomRotation;
	}

	[Header("Settings")]
	[SerializeField] private bool dontDestroyOnLoad;
	[SerializeField] private bool spawnOnlyOnBorders = true;
	[Header("References")]
	[SerializeField] private Collider boxCollider;
	[SerializeField] private List<Enemy> enemies;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;

			if (dontDestroyOnLoad)
				DontDestroyOnLoad(gameObject);
		}
		else if (Instance != this)
		{
			Destroy(Instance.gameObject);
			Instance = this;

			if (dontDestroyOnLoad)
				DontDestroyOnLoad(gameObject);
		}
	}

	#region Public Methods
	public static void SpawnRandom()
	{
		bool hasSpawned = false;
		while (!hasSpawned)
		{
			foreach (var enemy in Instance.enemies)
			{
				float random = Random.Range(0f, 1f);
				if (random < enemy.SpawnChance)
				{
					Instance.SpawnEnemy(enemy.Prefab, null, enemy.HasRandomRotation);
					hasSpawned = true;
					break;
				}
			}
		}
	}

	public static void Spawn(GameObject prefab, Vector3? pos = null)
    {
		Instance.SpawnEnemy(prefab, pos);
    }
	#endregion

	#region Private Methods
	private void SpawnEnemy(GameObject prefab, Vector3? pos = null, bool hasRandomRotation = true)
	{
		Bounds bounds = boxCollider.bounds;
		float offsetX = 0;
		float offsetZ = 0;

		Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 360f), 0);

		if (pos == null)
		{
			if (spawnOnlyOnBorders)
			{
				int direction = Random.Range(0, 2);
				if (direction == 0) // horizontal direction
				{
					int vertDirection = Random.Range(0, 2);
					offsetX = Random.Range(-bounds.extents.x, bounds.extents.x);
					offsetZ = vertDirection == 0 ? -bounds.extents.z : bounds.extents.z;
				}
				else // vertical direction
				{
					int horizDirection = Random.Range(0, 2);
					offsetZ = Random.Range(-bounds.extents.z, bounds.extents.z);
					offsetX = horizDirection == 0 ? -bounds.extents.x : bounds.extents.x;
				}
			}
			else
			{
				offsetX = Random.Range(-bounds.extents.x, bounds.extents.x);
				offsetZ = Random.Range(-bounds.extents.z, bounds.extents.z);
			}

			Vector3 randomPos = bounds.center + new Vector3(offsetX, 0, offsetZ);
			Instantiate(prefab, randomPos, hasRandomRotation ? randomRot : prefab.transform.rotation);
		}
		else
		{
			Instantiate(prefab, pos.Value, hasRandomRotation ? randomRot : prefab.transform.rotation);
		}

		GameManager.NumberOfEnemies++;
	}
	#endregion
}
