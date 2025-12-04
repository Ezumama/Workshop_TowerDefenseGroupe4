using UnityEngine;
using System.Collections;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

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
        public int[] allowedSpawnPoints; // indices des SpawnPoints autorisés pour cette wave
    }

    public Wave[] waves;
    public Transform[] airSpawnPoints;
    public float timeBetweenWaves = 2f;

    [Header("Référence au PathManager")]
    public PathManager pathManager;

    private int currentWave = 0;
    private int aliveEnemies = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(StartWaves());
    }

    public void RegisterEnemy()
    {
        aliveEnemies++;
    }

    public void UnregisterEnemy()
    {
        aliveEnemies--;
    }

    IEnumerator StartWaves()
    {
        while (currentWave < waves.Length)
        {
            yield return StartCoroutine(SpawnWave(waves[currentWave]));
            currentWave++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("🎉 Toutes les vagues terminées !");
    }

    IEnumerator SpawnWave(Wave wave)
    {
        // Si la wave ne spécifie rien, on utilise tous les spawn points
        int[] usableSpawnPoints = wave.allowedSpawnPoints.Length > 0
            ? wave.allowedSpawnPoints
            : Enumerable.Range(0, pathManager.spawnPointsData.Length).ToArray();

        foreach (var group in wave.enemies)
        {
            for (int i = 0; i < group.count; i++)
            {
                bool isFlying = group.enemyPrefab.GetComponent<EnemyFly>() != null;
                Transform spawn;
                GameObject obj;

                if (isFlying)
                {
                    // Ennemi volant → spawn aléatoire
                    spawn = airSpawnPoints[Random.Range(0, airSpawnPoints.Length)];
                    obj = Instantiate(group.enemyPrefab, spawn.position, spawn.rotation);
                }
                else
                {
                    // Choisir un SpawnPoint autorisé
                    int index = usableSpawnPoints[Random.Range(0, usableSpawnPoints.Length)];
                    PathManager.SpawnPointData spData = pathManager.spawnPointsData[index];

                    spawn = spData.spawnPointTransform;

                    obj = Instantiate(group.enemyPrefab, spawn.position, spawn.rotation);

                    // Passer le path au EnemyNav
                    EnemyNav nav = obj.GetComponent<EnemyNav>();
                    if (nav != null && spData.waypoints != null && spData.waypoints.Length > 0)
                    {
                        nav.SetPath(spData.waypoints);
                    }
                    else if (nav != null)
                    {
                        Debug.LogWarning($"[WaveManager] SpawnPoint {spawn.name} n'a pas de waypoints assignés !");
                    }
                }

                RegisterEnemy();
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
    }
}




