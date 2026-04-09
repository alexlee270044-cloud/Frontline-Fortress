using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 200f;
    private float currentHealth;
    public EnemyHealthBar healthBarUI;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBarUI != null)
        {
            healthBarUI.SetMaxHealth(maxHealth);
            healthBarUI.SetHealth(currentHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("TakeDamage 호출됨: " + damage);
        currentHealth -= damage;

        if (healthBarUI != null)
        {
            healthBarUI.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("적 사망");
        Destroy(gameObject);
    }
}
