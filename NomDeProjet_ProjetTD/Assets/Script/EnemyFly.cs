using System.Linq;
using UnityEngine;

public class EnemyFly : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypoint = 0;

    public float speed = 5f;
    private Health health;

    void Start()
    {
        health = GetComponent<Health>();

        GameObject wpParent = GameObject.Find("AirWaypoints");
        waypoints = wpParent.GetComponentsInChildren<Transform>().Skip(1).ToArray();

        if (waypoints.Length == 0)
        {
            Debug.LogError("❌ Aucun AirWaypoint trouvé !");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // Sécurité : éviter crash
        if (waypoints == null || waypoints.Length == 0 || currentWaypoint >= waypoints.Length)
            return;

        MoveTowardWaypoint();
    }

    void MoveTowardWaypoint()
    {
        Transform target = waypoints[currentWaypoint];

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist < 0.5f)
        {
            currentWaypoint++;

            // Si toutes les waypoints sont finies → destination atteinte
            if (currentWaypoint >= waypoints.Length)
            {
                ReachDestination();
            }
        }
    }

    void ReachDestination()
    {
        WaveManager.instance.UnregisterEnemy();
        //Destroy(gameObject);
    }
}



