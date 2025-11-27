using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

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



    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}

