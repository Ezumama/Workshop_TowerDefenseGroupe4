using UnityEngine;

public class Health : MonoBehaviour
{
    // 🔥 TRÈS IMPORTANT : Doit correspondre EXACTEMENT aux tags que vous utilisez sur vos prefabs ennemis
    [Header("Configuration des Tags")]
    public static readonly string[] EnemyTags = { "EnemyAir", "EnemyGround" };

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
        if (currentHealth <= 0) return; // Évite les calculs si l'objet est déjà mort

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Changé en public virtual pour que les enfants (Nexus, Enemy) puissent l'étendre
    public virtual void Die()
    {
        // 1. Déclenche l'événement de mort
        OnDie?.Invoke();

        // 2. Vérification et Désenregistrement de l'ennemi (Correction du blocage de vague)
        if (WaveManager.instance != null)
        {
            bool isEnemy = false;

            // Vérifie si l'objet qui meurt a un des tags d'ennemi définis
            foreach (string tag in EnemyTags)
            {
                if (gameObject.CompareTag(tag))
                {
                    isEnemy = true;
                    break;
                }
            }

            // Seulement les vrais ennemis sont désenregistrés
            if (isEnemy)
            {
                WaveManager.instance.UnregisterEnemy();
            }
        }

        // 3. Destruction de l'objet (Nexus, Ennemi, etc.)
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

