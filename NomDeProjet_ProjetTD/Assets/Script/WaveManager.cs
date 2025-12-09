using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    // --- Configurations de la Vague ---
    [System.Serializable]
    public class EnemySpawn
    {
        public GameObject enemyPrefab;
        public int count = 5;
        public float spawnInterval = 1f;
    }

    [System.Serializable]
    public class Wave
    {
        public EnemySpawn[] enemies;
        [Tooltip("Indices des SpawnPoints autorisés (dans le tableau 'spawnPointsData' du PathManager). Laisser vide pour TOUS.")]
        public int[] allowedSpawnPoints;
    }

    public Wave[] waves;
    [Tooltip("Les 4 points de spawn pour les ennemis volants.")]
    public Transform[] airSpawnPoints;
    public float timeBetweenWaves = 2f;

    [Header("Référence au PathManager")]
    // **Correction CS0103** : Assure que la référence est là.
    public PathManager pathManager;

    // --- État du Manager ---
    private int currentWave = 0;
    // **Correction CS0103** : Utilisation du nom correct 'aliveEnemies'.
    private int aliveEnemies = 0;


    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
    }

    void Start()
    {
        // Vérification des dépendances cruciales
        if (pathManager == null)
        {
            Debug.LogError("[WaveManager] Le PathManager n'est pas assigné ! Le système de vagues ne peut pas démarrer.");
            enabled = false;
            return;
        }

        StartCoroutine(StartWaves());
    }

    public void RegisterEnemy()
    {
        aliveEnemies++;
    }

    public void UnregisterEnemy()
    {
        aliveEnemies--;

        // Logique de fin de vague
        if (aliveEnemies <= 0 && currentWave < waves.Length)
        {
            // La dernière vague n'est pas forcément terminée, mais cela permet de passer à la suite si tous les ennemis sont morts.
            // Une logique plus robuste impliquerait un décompte des ennemis à spawner vs ennemis spammés et morts.
        }
    }

    IEnumerator StartWaves()
    {
        // Utilisation du nom correct 'currentWave'
        while (currentWave < waves.Length)
        {
            Debug.Log($"🚀 Début de la vague {currentWave + 1}...");
            yield return StartCoroutine(SpawnWave(waves[currentWave]));

            // Attendre que tous les ennemis soient morts avant de passer à la vague suivante (logique classique de TD)
            yield return new WaitUntil(() => aliveEnemies <= 0);

            currentWave++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("🎉 Toutes les vagues terminées !");
    }

    IEnumerator SpawnWave(Wave wave)
    {
        // 1. Déterminer les indices des SpawnPoints utilisables
        int[] usableSpawnIndexes = wave.allowedSpawnPoints.Length > 0
            ? wave.allowedSpawnPoints
            : Enumerable.Range(0, pathManager.spawnPointsData.Length).ToArray();

        // S'assurer qu'il y a des chemins configurés et utilisables
        if (usableSpawnIndexes.Length == 0)
        {
            Debug.LogError("[WaveManager] Aucun SpawnPoint autorisé ou configuré dans le PathManager.");
            yield break;
        }

        foreach (var group in wave.enemies)
        {
            for (int i = 0; i < group.count; i++)
            {
                // Tenter de récupérer un script de mouvement pour déterminer le type
                bool isFlying = group.enemyPrefab.GetComponent<EnemyAir>() != null;
                bool isGrounded = group.enemyPrefab.GetComponent<EnemyNav>() != null;

                Transform spawn = null;
                GameObject obj = null;
                bool enemySpawned = false;

                if (isFlying)
                {
                    // Ennemi volant → spawn aléatoire sur un AirSpawnPoint
                    if (airSpawnPoints != null && airSpawnPoints.Length > 0)
                    {
                        spawn = airSpawnPoints[Random.Range(0, airSpawnPoints.Length)];
                        obj = Instantiate(group.enemyPrefab, spawn.position, spawn.rotation);
                        // Les ennemis volants utilisent leur propre logique EnemyAir
                        enemySpawned = true;
                    }
                }
                else if (isGrounded)
                {
                    // --- 1. Choisir le SpawnPoint autorisé ---
                    int index = usableSpawnIndexes[Random.Range(0, usableSpawnIndexes.Length)];

                    // Assure-toi que SpawnPointData est bien une classe publique dans PathManager
                    PathManager.SpawnPointData spData = pathManager.spawnPointsData[index];

                    if (spData.waypoints == null || spData.waypoints.Length == 0)
                    {
                        //Debug.LogWarning($"SpawnPoint {spData.spawnPointTransform.name} n'a pas de Nœuds de chemin (Node) configurés !");
                        continue;
                    }

                    // --- 2. Choisir aléatoirement une seule Node (le chemin) ---
                    Transform selectedNode = spData.waypoints[Random.Range(0, spData.waypoints.Length)];

                    // --- 3. Extraire TOUS les points enfants de cette Node ---
                    List<Transform> pathPoints = new List<Transform>();

                    // Récupère les vrais points (Point 1, Point 2, ...) dans l'ordre de la hiérarchie
                    foreach (Transform point in selectedNode)
                    {
                        // S'assurer que le Transform est un point de passage et non un autre parent
                        pathPoints.Add(point);
                    }

                    if (pathPoints.Count == 0)
                    {
                        //Debug.LogWarning($"Le Nœud sélectionné ({selectedNode.name}) ne contient aucun point de chemin !");
                        continue;
                    }

                    // Point de spawn
                    spawn = spData.spawnPointTransform;

                    // --- 4. Instancier et assigner le chemin ---
                    obj = Instantiate(group.enemyPrefab, spawn.position, spawn.rotation);

                    EnemyNav nav = obj.GetComponent<EnemyNav>();
                    if (nav != null)
                    {
                        nav.SetPath(pathPoints.ToArray());
                        enemySpawned = true;
                    }
                    else
                    {
                        //Debug.LogError($"L'ennemi au sol {group.enemyPrefab.name} n'a pas de composant EnemyNav !");
                    }
                }

                if (enemySpawned)
                {
                    RegisterEnemy();
                }

                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
    }
}





