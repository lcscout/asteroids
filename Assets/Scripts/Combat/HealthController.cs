using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
	#region Fields
	[Header("Settings")]
	[SerializeField] private int maxHealth;
	[Header("Debug")]
	[SerializeField] private int currentHealth;
	#endregion

	#region Properties
	public int CurrentHealth => currentHealth;
	public int MaxHealth => maxHealth;
	#endregion

	#region Unity Messages
	private void Awake()
	{
		currentHealth = maxHealth;
	}
	#endregion

	#region Public Methods
	public void HealOrAdd(int heal)
	{
		currentHealth += heal;

		if (currentHealth > maxHealth)
			maxHealth = currentHealth;
	}

	public void Heal(int heal)
	{
		if (currentHealth < maxHealth)
		{
			currentHealth += heal;
		}

		// um fix pra caso a vida passar do máximo
		if (currentHealth > maxHealth)
			currentHealth = maxHealth;
	}

	public void TakeDamage(int damage, Action onDeath = null)
	{
		currentHealth -= damage;

		if (currentHealth <= 0)
			onDeath?.Invoke();
	}
	#endregion

	#region Private Methods
	#endregion
}
