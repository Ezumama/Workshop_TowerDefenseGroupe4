using UnityEngine;
using System.Collections.Generic;
using System; // Nécessaire pour [Serializable]

public class PathManager : MonoBehaviour
{
    public static PathManager instance;

    // --- CLASSE INTERNE POUR LA CONFIGURATION DU CHEMIN ---
    // DOIT ÊTRE PUBLIC pour que WaveManager puisse y accéder (PathManager.SpawnPointData)
    [Serializable]
    public class SpawnPointData
    {
        [Tooltip("Point de départ de l'ennemi (où il est instancié).")]
        public Transform spawnPointTransform;

        [Tooltip("Liste des Nœuds Pères (Node 1A, Node 1B, etc.). Chaque Node est un chemin complet. Un seul sera choisi au hasard.")]
        // Ce tableau contient les Transforms des objets 'Node X' qui sont les parents des vrais points (Point 1, Point 2, etc.)
        public Transform[] waypoints;
    }

    // --- CHAMP PRINCIPAL ---
    [Tooltip("Liste de tous les SpawnPoints disponibles et de leurs chemins associés.")]
    public SpawnPointData[] spawnPointsData;

    // --- Logique du Manager ---

    void Awake()
    {
        if (instance != null)
        {
            //Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Le PathManager n'a plus besoin d'une méthode pour récupérer un chemin,
    // car le WaveManager gère maintenant l'extraction des points enfants
    // directement en utilisant le champ 'spawnPointsData'.
}



