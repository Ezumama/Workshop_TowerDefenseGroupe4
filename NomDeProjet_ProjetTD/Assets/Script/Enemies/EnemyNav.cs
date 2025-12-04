using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyNav : MonoBehaviour
{
    private NavMeshAgent agent;
    private Health health;

    private Transform[] currentPath;
    private int currentIndex = 0;

    private bool pathAssigned = false;

    public Transform[] pathPoints; // utilisé par SetPath()

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();

        if (agent == null)
        {
            Debug.LogError("[EnemyNav] Pas de NavMeshAgent sur le prefab !");
            enabled = false;
            return;
        }

        // Si path déjà assigné avant Start(), on initialise
        if (pathAssigned && pathPoints != null && pathPoints.Length > 0)
        {
            currentPath = pathPoints;
            currentIndex = 0;
            agent.SetDestination(currentPath[0].position);
            Debug.Log($"[EnemyNav] {name} utilise un path assigné via SetPath(), longueur = {currentPath.Length}");
        }
    }


    void Update()
    {
        if (!agent || currentPath == null || currentPath.Length == 0) return;
        if (agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentIndex++;

            if (currentIndex < currentPath.Length)
            {
                agent.SetDestination(currentPath[currentIndex].position);
            }
            else
            {
                ReachDestination();
            }
        }
    }

public void SetPath(Transform[] points)
{
    pathPoints = points;
    pathAssigned = true;

    // Si Start() est déjà passé, on initialise le chemin maintenant
    if (agent != null && pathPoints.Length > 0)
    {
        currentPath = pathPoints;
        currentIndex = 0;
        agent.SetDestination(currentPath[0].position);
    }
}

    void ReachDestination()
    {
        if (WaveManager.instance != null)
            WaveManager.instance.UnregisterEnemy();
        Destroy(gameObject);
    }
}



