using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject); 
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }




    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}

