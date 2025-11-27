using UnityEngine;
using System.Collections;

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
    }
    
    public Wave[] waves;
    public Transform[] spawnPoints;
    public Transform[] airSpawnPoint;
    public float timeBetweenWaves = 2f;

    private int currentWave = 0;
    private int aliveEnemies = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        //else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(StartWaves());
    }

    public void RegisterEnemy()
    {
        aliveEnemies++;
        Debug.Log("Spawn → Alive: " + aliveEnemies);
    }

    public void UnregisterEnemy()
    {
        aliveEnemies--;
        Debug.Log("Death → Alive: " + aliveEnemies);
    }

    IEnumerator StartWaves()
    {
        while (currentWave < waves.Length)
        {
            yield return StartCoroutine(SpawnWave(waves[currentWave]));

            // Attendre la fin des ennemis

            //yield return new WaitUntil(() => aliveEnemies <= 0);

            currentWave++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("🎉 Toutes les vagues terminées !");
    }

    IEnumerator SpawnWave(Wave wave)
    {
        foreach (var group in wave.enemies)
        {
            for (int i = 0; i < group.count; i++)
            {
                // On regarde si c'est un ennemi volant
                bool isFlying = group.enemyPrefab.GetComponent<EnemyFly>() != null;

                Transform spawn;

                if (isFlying)
                {
                    // Spawn dans les AirSpawnPoint
                    spawn = airSpawnPoint[Random.Range(0, airSpawnPoint.Length)];
                }
                else
                {
                    // Spawn dans les SpawnPoint classiques
                    spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
                }

                Instantiate(group.enemyPrefab, spawn.position, spawn.rotation);
                RegisterEnemy();

                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
    }

}


