using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [System.Serializable]
    public class Wave
    {
        public GameObject enemyPrefab;
        public int count = 5;
        public float spawnInterval = 1f;
    }

    public Wave[] waves;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 2f;

    private int currentWave = 0;
    private int aliveEnemies = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(StartWaves());
    }

    public void RegisterEnemy()
    {
        aliveEnemies++;
        Debug.Log("Enemy spawned. Alive: " + aliveEnemies);
    }

    public void UnregisterEnemy()
    {
        aliveEnemies--;
        Debug.Log("Enemy died. Alive: " + aliveEnemies);
    }

    IEnumerator StartWaves()
    {
        while (currentWave < waves.Length)
        {
            yield return StartCoroutine(SpawnWave(waves[currentWave]));

            // Attendre que tous les ennemis meurent avant la prochaine vague
            yield return new WaitUntil(() => aliveEnemies <= 0);

            currentWave++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("🎉 Toutes les vagues terminées !");
    }

    IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.count; i++)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(wave.enemyPrefab, spawn.position, spawn.rotation);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }
}

