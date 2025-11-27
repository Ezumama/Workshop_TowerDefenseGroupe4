using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNav : MonoBehaviour
{
    public Transform[] waypoints; // chemin à suivre
    private int currentWaypoint = 0;

    private NavMeshAgent agent;
    private Health health;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();

        GameObject wpParent = GameObject.Find("Waypoints");
        waypoints = wpParent.GetComponentsInChildren<Transform>();

        // retirer le parent
        waypoints = waypoints.Skip(1).ToArray();

        agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        if (waypoints.Length == 0 || agent == null)
            return;

        // Si proche de la waypoint actuelle
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypoint++;
            if (currentWaypoint < waypoints.Length)
            {
                agent.SetDestination(waypoints[currentWaypoint].position);
            }
            else
            {
                ReachDestination();
            }
        }
    }

    void ReachDestination()
    {
        // Ennemi atteint la fin → infliger dégâts à la base si nécessaire
        WaveManager.instance.UnregisterEnemy();
        Destroy(gameObject);
    }

    public void TakeDamage(float dmg)
    {
        if (health != null)
            health.TakeDamage(dmg);
    }

    public void Die()
    {
        WaveManager.instance.UnregisterEnemy();
        Destroy(gameObject);
    }
}

