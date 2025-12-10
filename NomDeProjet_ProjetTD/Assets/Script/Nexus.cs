using UnityEngine;
using System.Collections.Generic;

public class Nexus : MonoBehaviour
{
    [Header("Dégâts infligés aux ennemis")]
    [Tooltip("Dégâts continus que le Nexus inflige aux ennemis dans son rayon.")]
    public float damageToEnemiesPerSecond = 5f;

    // Pas besoin de targetTags ici, nous utilisons Health.EnemyTags

    private Health nexusHealth;
    // Liste des ennemis capables d'attaquer le Nexus (utilisant EnemyNexusDamage)
    private List<EnemyNexusDamage> attackersInRange = new List<EnemyNexusDamage>();

    // Liste des ennemis à frapper par le Nexus (utilisant Health, pour les dégâts continus)
    private List<Health> enemiesToHit = new List<Health>();

    void Start()
    {
        nexusHealth = GetComponent<Health>();
    }

    void Update()
    {
        // Nettoie automatiquement les ennemis détruits des deux listes
        attackersInRange.RemoveAll(a => a == null);
        enemiesToHit.RemoveAll(e => e == null);


        // --- 1. Le Nexus inflige des dégâts aux ennemis ---
        // Dégâts constants (DoT) à tous les ennemis à portée.
        foreach (var enemy in enemiesToHit)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(damageToEnemiesPerSecond * Time.deltaTime);
            }
        }

        // --- 2. Le Nexus reçoit des dégâts des ennemis ---
        // Chaque ennemi gère son propre taux et cooldown d'attaque.
        if (nexusHealth.IsAlive())
        {
            foreach (var attacker in attackersInRange)
            {
                if (attacker != null)
                {
                    // Appelle la fonction de l'ennemi qui vérifie son propre cooldown
                    attacker.TryDamageNexus(nexusHealth);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // --- 1. Enregistrement pour les dégâts infligés par le Nexus (Nexus -> Ennemis) ---
        if (IsTargetedEnemy(other.gameObject))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null && enemyHealth != nexusHealth && !enemiesToHit.Contains(enemyHealth))
            {
                enemiesToHit.Add(enemyHealth);
            }
        }

        // --- 2. Enregistrement pour les dégâts reçus par le Nexus (Ennemis -> Nexus) ---
        EnemyNexusDamage enemyAttacker = other.GetComponent<EnemyNexusDamage>();
        if (enemyAttacker != null && !attackersInRange.Contains(enemyAttacker))
        {
            attackersInRange.Add(enemyAttacker);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Retrait des ennemis qui sortent du rayon
        Health enemyHealth = other.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemiesToHit.Remove(enemyHealth);
        }

        EnemyNexusDamage enemyAttacker = other.GetComponent<EnemyNexusDamage>();
        if (enemyAttacker != null)
        {
            attackersInRange.Remove(enemyAttacker);
        }
    }

    // Vérifie si le GameObject a un des tags d'ennemi définis dans Health.cs
    private bool IsTargetedEnemy(GameObject obj)
    {
        // Utilise la liste statique des tags ennemis de la classe Health pour la cohérence
        foreach (string tag in Health.EnemyTags)
        {
            if (obj.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }
}

