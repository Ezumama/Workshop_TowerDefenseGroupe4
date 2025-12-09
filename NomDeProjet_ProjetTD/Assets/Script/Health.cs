using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public float maxHealth = 100f;
    public float currentHealth;

    // Ajout d'un événement pour que d'autres scripts puissent réagir à la mort
    public event System.Action OnDie;

    void Awake()
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

    // Changé en public virtual pour que les enfants (Nexus, Enemy) puissent l'étendre
    public virtual void Die()
    {
        OnDie?.Invoke(); // Déclenche l'événement
        Destroy(gameObject);
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsAlive() => currentHealth > 0;
}

