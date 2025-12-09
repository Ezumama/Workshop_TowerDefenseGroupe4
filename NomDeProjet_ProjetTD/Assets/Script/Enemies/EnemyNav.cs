using UnityEngine;
using UnityEngine.AI;
using System.Linq;

// EnemyNav n'a plus besoin d'hériter de EnemyBase (il l'utilisera si NavMesh est désactivé)

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNav : EnemyBase // Hérite maintenant de EnemyBase
{
    private NavMeshAgent agent;

    private Transform[] currentPath;
    private int currentIndex = 0;
    private bool pathAssigned = false;

    // Pas besoin de pathPoints public, le path est passé via SetPath

    protected override void Start() // Utilise protected override
    {
        base.Start(); // Appelle le Start d'EnemyBase

        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("[EnemyNav] NavMeshAgent manquant !");
            enabled = false;
            return;
        }

        if (pathAssigned && currentPath != null && currentPath.Length > 0)
        {
            // La destination finale devrait être le Nexus si c'est le dernier point
            agent.SetDestination(currentPath[0].position);
        }
    }

    void Update()
    {
        if (!agent || currentPath == null || currentPath.Length == 0) return;
        if (agent.pathPending) return;

        // Vrai si l'ennemi est proche du point actuel
        // ET si l'agent a terminé le calcul de son chemin (remainingDistance est une bonne approximation)
        if (currentIndex < currentPath.Length && agent.remainingDistance <= agent.stoppingDistance)
        {
            // 1. L'ennemi a atteint le point currentIndex
            Debug.Log($"[EnemyNav] Atteint le point {currentPath[currentIndex].name}. Passage au suivant.");
            currentIndex++; // On passe au point suivant

            if (currentIndex < currentPath.Length)
            {
                // 2. Il y a un point suivant : On assigne la nouvelle destination.
                agent.SetDestination(currentPath[currentIndex].position);
                Debug.Log($"[EnemyNav] Nouvelle destination: {currentPath[currentIndex].name}.");
            }
            else
            {
                // 3. Tous les points sont atteints (Nexus)
                ReachDestination();
            }
        }
    }

    // Le path doit être assigné AVANT Start() si possible
    public void SetPath(Transform[] points)
    {
        currentPath = points;
        pathAssigned = true;

        // Si Start() est déjà passé (spawn après un certain temps, ou si besoin)
        if (agent != null && currentPath.Length > 0)
        {
            currentIndex = 0;
            agent.SetDestination(currentPath[0].position);
            Debug.Log($"[EnemyNav] {name} commence sur un path de {currentPath.Length} points. Prochain point: {currentPath[0].name}.");
        }
    }

    protected override void ReachDestination()
    {
        // TODO: Infliger des dégâts au Nexus ici si tu veux une punition directe

        base.ReachDestination(); // S'enregistre auprès du WaveManager et se détruit
    }
}




