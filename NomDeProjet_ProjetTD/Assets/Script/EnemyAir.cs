using UnityEngine;
using System.Linq;

public class EnemyAir : EnemyBase
{
    [SerializeField] private float speed = 5f;
    private Transform nexusTarget; // La cible est le Nexus (le point AirWaypoint)

    protected override void Start()
    {
        base.Start();

        // Trouver la cible (le Point 1 sous AirWaypoints)
        GameObject wpParent = GameObject.Find("AirWaypoints");
        if (wpParent != null && wpParent.transform.childCount > 0)
        {
            nexusTarget = wpParent.transform.GetChild(0); // Le "Point 1"
        }

        if (nexusTarget == null)
        {
            Debug.LogError("❌ AirWaypoint/Nexus Target non trouvé !");
            enabled = false;
        }
    }

    void Update()
    {
        if (nexusTarget == null) return;

        // Mouvement direct vers le Nexus
        Vector3 dir = (nexusTarget.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // Si proche de la cible (Nexus)
        if (Vector3.Distance(transform.position, nexusTarget.position) < 0.5f)
        {
            ReachDestination();
        }
    }

    protected override void ReachDestination()
    {
        // TODO: Infliger des dégâts au Nexus ici si tu veux une punition directe
        base.ReachDestination();
    }
}
