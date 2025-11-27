using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyNav : MonoBehaviour
{
    private NavMeshAgent agent;
    private Health health;

    // Current "node" (container qui référence un segment / chemin)
    private WaypointNode currentNode;
    private Transform[] currentPath;    // waypoints du segment choisi
    private int currentIndex = 0;

    [Tooltip("Si true, le spawn choisira un WaypointNode de départ aléatoire.")]
    public bool randomStartNode = true;

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

        // Récupère tous les WaypointNode de la scène
        var nodes = GameObject.FindObjectsOfType<WaypointNode>();
        if (nodes == null || nodes.Length == 0)
        {
            Debug.LogError("[EnemyNav] Aucun WaypointNode trouvé dans la scène !");
            enabled = false;
            return;
        }

        // Choisir le node de départ (aléatoire ou le premier)
        currentNode = randomStartNode ? nodes[Random.Range(0, nodes.Length)] : nodes[0];
        if (currentNode == null)
        {
            Debug.LogError("[EnemyNav] currentNode null après choix !");
            enabled = false;
            return;
        }

        // Choisir un chemin initial dans ce node (gère si node.nextPaths vide)
        ChooseNewPathFromNode(currentNode);
    }

    void Update()
    {
        if (!agent || currentPath == null || currentPath.Length == 0) return;

        // si le chemin est en attente, ne rien faire
        if (agent.pathPending) return;

        // si on est proche de la cible actuelle
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentIndex++;

            if (currentIndex < currentPath.Length)
            {
                agent.SetDestination(currentPath[currentIndex].position);
            }
            else
            {
                // fin du segment : choisir nouvel embranchement à partir du node courant (si possible)
                ChooseNewNodeAndPath();
            }
        }
    }

    void ChooseNewNodeAndPath()
    {
        // Si le node courant a des embranchements, on en choisit un aléatoirement
        if (currentNode != null && currentNode.nextPaths != null && currentNode.nextPaths.Length > 0)
        {
            Transform chosen = currentNode.nextPaths[Random.Range(0, currentNode.nextPaths.Length)];
            if (chosen == null)
            {
                Debug.LogWarning("[EnemyNav] chosen nextPath était null, tentative de rester sur le même node.");
                ReachDestination();
                return;
            }

            // si l'élément choisi contient un WaypointNode (ex: point vers un autre node), on le récupère
            WaypointNode nextNode = chosen.GetComponent<WaypointNode>();
            if (nextNode != null)
            {
                currentNode = nextNode;
            }
            else
            {
                // sinon on crée un faux node temporaire (conteneur de chemin)
                currentNode = chosen.gameObject.AddComponent<WaypointNodeMarker>();
            }

            ChooseNewPathFromNode(currentNode);
        }
        else
        {
            // pas d'embranchement : fin de chemin
            ReachDestination();
        }
    }

    void ChooseNewPathFromNode(WaypointNode node)
    {
        if (node == null)
        {
            Debug.LogError("[EnemyNav] Node null dans ChooseNewPathFromNode");
            ReachDestination();
            return;
        }

        // Récupère la liste des waypoints enfants du node choisi (skip parent)
        currentPath = node.GetComponentsInChildren<Transform>()
                          .Where(t => t != node.transform)
                          .ToArray();

        if (currentPath.Length == 0)
        {
            Debug.LogWarning("[EnemyNav] Le node '" + node.name + "' n'a pas de waypoints enfants -> fin de chemin.");
            ReachDestination();
            return;
        }

        currentIndex = 0;
        agent.SetDestination(currentPath[0].position);

        Debug.Log($"[EnemyNav] {gameObject.name} démarre sur node '{node.name}', path len={currentPath.Length}");
    }

    void ReachDestination()
    {
        // arriver à la fin : notifier et détruire
        if (WaveManager.instance != null)
            WaveManager.instance.UnregisterEnemy();
        //Destroy(gameObject);
    }

    // marqueur minimal (utilisé si un transform ne contient pas WaypointNode)
    private class WaypointNodeMarker : WaypointNode { }
}



