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

        // 🚨 SI AUCUN PATH N’A ÉTÉ DONNÉ PAR LE SPAWNPOINT 🚨
        if (!pathAssigned || pathPoints == null || pathPoints.Length == 0)
        {
            Debug.LogError($"[EnemyNav] Aucun path assigné au spawn !", this);
            enabled = false;
            return;
        }

        // sinon on utilise le path donné par le SpawnPoint
        currentPath = pathPoints;
        currentIndex = 0;

        agent.SetDestination(currentPath[0].position);

        Debug.Log($"[EnemyNav] {name} utilise un path assigné via SetPath(), longueur = {currentPath.Length}");
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
        pathAssigned = true; // important ! sinon Start() override tout
    }

    void ReachDestination()
    {
        if (WaveManager.instance != null)
            WaveManager.instance.UnregisterEnemy();
        Destroy(gameObject);
    }
}



