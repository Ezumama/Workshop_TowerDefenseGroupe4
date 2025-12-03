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
                bool isFlying = group.enemyPrefab.GetComponent<EnemyFly>() != null;

                Transform spawn;

                if (isFlying)
                    spawn = airSpawnPoint[Random.Range(0, airSpawnPoint.Length)];
                else
                    spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

                GameObject enemyObj = Instantiate(group.enemyPrefab, spawn.position, spawn.rotation);

                // 🔥 RÉCUPÉRER LE PATH ASSOCIÉ AU SPAWNPOINT 🔥
                SpawnPointData spData = spawn.GetComponent<SpawnPointData>();

                if (!isFlying && spData != null)
                {
                    EnemyNav nav = enemyObj.GetComponent<EnemyNav>();

                    if (nav != null)
                    {
                        // On choisit Node1A ou Node1B aléatoirement
                        Transform selectedNode = spData.nodes[Random.Range(0, spData.nodes.Length)];

                        // On récupère tous les points enfant
                        Transform[] points = new Transform[selectedNode.childCount];
                        for (int p = 0; p < points.Length; p++)
                            points[p] = selectedNode.GetChild(p);

                        nav.SetPath(points);
                    }
                }

                RegisterEnemy();
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
    }


}


