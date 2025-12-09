using UnityEngine;

// Assure qu'un composant Health est toujours présent
[RequireComponent(typeof(Health))]
public abstract class EnemyBase : MonoBehaviour
{
    protected Health health;

    protected virtual void Start()
    {
        health = GetComponent<Health>();

        // Abonne-toi à l'événement de mort du Health
        health.OnDie += OnDeath;

        // Enregistre l'ennemi au spawn
        if (WaveManager.instance != null)
            WaveManager.instance.RegisterEnemy();
    }

    // Méthode appelée lorsque l'ennemi atteint sa destination (le Nexus)
    protected virtual void ReachDestination()
    {
        // Logique spécifique : par exemple, infliger des dégâts au Nexus
        // et se détruire. Le Nexus est censé être la dernière 'cible' du chemin.

        // Désinscription/Destruction
        UnregisterAndDestroy();
    }

    // Méthode appelée à la mort (dégâts, ou ReachDestination)
    private void OnDeath()
    {
        // Se désabonne pour éviter des appels nuls après la destruction
        health.OnDie -= OnDeath;
        UnregisterAndDestroy();
    }

    protected void UnregisterAndDestroy()
    {
        if (WaveManager.instance != null)
            WaveManager.instance.UnregisterEnemy();

        // Le Health.Die() détruit déjà le GameObject, mais on peut le faire ici aussi si besoin
        if (health.IsAlive() && gameObject != null) // Vérifie si pas déjà détruit par Die()
            //Destroy(gameObject);
            return;
    }
}
