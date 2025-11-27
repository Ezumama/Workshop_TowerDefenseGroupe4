using UnityEngine;
using System.Collections.Generic;

public class Nexus : MonoBehaviour
{
    public float damageToEnemiesPerSecond = 5f;
    public float damageToNexusPerSecond = 2f;

    private Health nexusHealth;
    private List<Health> enemiesInRange = new List<Health>();

    void Start()
    {
        nexusHealth = GetComponent<Health>();
    }

    void Update()
    {
        // 🔥 Nettoie automatiquement les ennemis morts ou supprimés
        enemiesInRange.RemoveAll(e => e == null);

        // Infliger des dégâts aux ennemis
        foreach (var enemy in enemiesInRange)
        {
            enemy.TakeDamage(damageToEnemiesPerSecond * Time.deltaTime);
        }

        // Infliger des dégâts au Nexus 
        if (enemiesInRange.Count > 0)
        {
            float totalDamage = damageToNexusPerSecond * enemiesInRange.Count * Time.deltaTime;
            nexusHealth.TakeDamage(totalDamage);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Health enemyHealth = other.GetComponent<Health>();

        if (enemyHealth != null && enemyHealth != nexusHealth)
        {
            enemiesInRange.Add(enemyHealth);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Health enemyHealth = other.GetComponent<Health>();

        if (enemyHealth != null)
        {
            enemiesInRange.Remove(enemyHealth);
        }
    }
}


