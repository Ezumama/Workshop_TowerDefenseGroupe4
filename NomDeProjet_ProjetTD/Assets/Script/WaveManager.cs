using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    // --- Structures de Configuration (Inch.) ---

    [System.Serializable]
    public class EnemyBatch
    {
        public GameObject enemyPrefab;
        public int count = 5;
        public float spawnInterval = 1f;
    }

    [System.Serializable]
    public class PathGroup
    {
        [Tooltip("Index du SpawnPoint dans le PathManager.spawnPointsData (e.g., 0 pour Path 1, 1 pour Path 2, etc.)")]
        public int pathManagerIndex;

        [Tooltip("Les lots d'ennemis qui vont spawner sur ce chemin.")]
        public EnemyBatch[] enemies;
    }

    [System.Serializable]
    public class Wave
    {
        public PathGroup[] pathGroups;
    }

    // --- Variables du Manager (Inch.) ---

    public Wave[] waves;
    [Tooltip("Les points de spawn pour les ennemis volants. DOIVENT ÊTRE DANS LE MÊME ORDRE QUE LES CHEMINS DU PATHMANAGER.")]
    public Transform[] airSpawnPoints;
    public float timeBetweenWaves = 2f;

    [Header("Référence au PathManager")]
    public PathManager pathManager;

    // --- État du Manager ---
    private int currentWave = 0;
    private int aliveEnemies = 0;
    private bool isSpawning = false; // 🔥 NOUVEAU : Indique si l'instanciation est en cours


    void Awake()
    {
        if (instance != null) { return; }
        instance = this;
    }

    void Start()
    {
        if (pathManager == null)
        {
            Debug.LogError("[WaveManager] Le PathManager n'est pas assigné ! Le système de vagues ne peut pas démarrer.");
            enabled = false;
            return;
        }

        StartCoroutine(StartWaves());
    }

    // --- Gestion des Ennemis ---

    public void RegisterEnemy()
    {
        aliveEnemies++;
    }

    public void UnregisterEnemy()
    {
        // 🔥 CORRECTION : Vérification pour éviter de décrémenter sous zéro.
        if (aliveEnemies > 0)
        {
            aliveEnemies--;
        }
        else
        {
            // Ceci peut aider au débogage si un ennemi tente de se désenregistrer deux fois.
            Debug.LogWarning("[WaveManager] Tentative de désenregistrer un ennemi alors que le compteur est déjà à zéro.");
        }
    }

    // --- Logique des Vagues ---

    IEnumerator StartWaves()
    {
        while (currentWave < waves.Length)
        {
            Debug.Log($"🚀 Début de la vague {currentWave + 1}...");

            isSpawning = true; // Début de l'instanciation
            yield return StartCoroutine(SpawnWave(waves[currentWave]));
            isSpawning = false; // Fin de l'instanciation (toutes les coroutines SpawnPathGroup ont terminé)


            // 🔥 CORRECTION CRUCIALE : On attend que le spawn soit terminé ET que tous les ennemis soient morts.
            yield return new WaitUntil(() => !isSpawning && aliveEnemies <= 0);
            // Si aliveEnemies reste > 0 ici, le problème est dans les scripts EnemyNav/EnemyAir.

            currentWave++;
            Debug.Log($"⏸️ Pause avant la vague {currentWave + 1}");
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("🎉 Toutes les vagues terminées !");
    }

    IEnumerator SpawnWave(Wave wave)
    {
        List<Coroutine> pathSpawners = new List<Coroutine>();

        foreach (var pathGroup in wave.pathGroups)
        {
            Coroutine spawner = StartCoroutine(SpawnPathGroup(pathGroup));
            pathSpawners.Add(spawner);
        }

        foreach (Coroutine spawner in pathSpawners)
        {
            yield return spawner;
        }
    }

    IEnumerator SpawnPathGroup(PathGroup pathGroup)
    {
        int pathIndex = pathGroup.pathManagerIndex;

        // --- 1. Récupérer les données du chemin (Path) et des points ---

        if (pathIndex < 0 || pathIndex >= pathManager.spawnPointsData.Length)
        {
            Debug.LogError($"[WaveManager] Index de chemin invalide : {pathIndex}. Assurez-vous qu'il existe dans le PathManager.");
            yield break;
        }

        PathManager.SpawnPointData spData = pathManager.spawnPointsData[pathIndex];
        Transform spawnPointTransform = spData.spawnPointTransform;

        if (spData.waypoints == null || spData.waypoints.Length == 0)
        {
            Debug.LogWarning($"[WaveManager] Le SpawnPoint {spawnPointTransform.name} (index {pathIndex}) n'a pas de Nœuds de chemin (waypoints) configurés !");
            yield break;
        }

        Transform[] availableNodes = spData.waypoints;


        // --- 2. Parcourir tous les lots d'ennemis pour ce chemin et les spawner ---

        foreach (var batch in pathGroup.enemies)
        {
            for (int i = 0; i < batch.count; i++)
            {
                bool isFlying = batch.enemyPrefab.GetComponent<EnemyAir>() != null;
                bool isGrounded = batch.enemyPrefab.GetComponent<EnemyNav>() != null;

                GameObject obj = null;
                bool enemySpawned = false;

                if (isFlying)
                {
                    // Logique pour Volants (utilise l'index du chemin)
                    if (airSpawnPoints != null && airSpawnPoints.Length > 0)
                    {
                        Transform airSpawn = null;

                        if (pathIndex >= 0 && pathIndex < airSpawnPoints.Length)
                        {
                            airSpawn = airSpawnPoints[pathIndex];
                        }
                        else
                        {
                            airSpawn = airSpawnPoints[0];
                        }

                        if (airSpawn != null)
                        {
                            obj = Instantiate(batch.enemyPrefab, airSpawn.position, airSpawn.rotation);
                            enemySpawned = true;
                        }
                    }
                }
                else if (isGrounded)
                {
                    // Logique pour les ennemis au sol (EnemyNav, chemin aléatoire)

                    Transform selectedNode = availableNodes[Random.Range(0, availableNodes.Length)];

                    List<Transform> pathPoints = new List<Transform>();
                    foreach (Transform point in selectedNode)
                    {
                        pathPoints.Add(point);
                    }

                    if (pathPoints.Count == 0)
                    {
                        Debug.LogWarning($"Le Nœud sélectionné ({selectedNode.name}) ne contient aucun point de chemin !");
                        continue;
                    }

                    obj = Instantiate(batch.enemyPrefab, spawnPointTransform.position, spawnPointTransform.rotation);

                    EnemyNav nav = obj.GetComponent<EnemyNav>();
                    if (nav != null)
                    {
                        nav.SetPath(pathPoints.ToArray());
                        enemySpawned = true;
                    }
                    else
                    {
                        Debug.LogError($"L'ennemi au sol {batch.enemyPrefab.name} n'a pas de composant EnemyNav !");
                    }
                }

                if (enemySpawned)
                {
                    RegisterEnemy();
                }

                yield return new WaitForSeconds(batch.spawnInterval);
            }
        }
    }
}




