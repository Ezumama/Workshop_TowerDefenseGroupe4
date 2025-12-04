using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager instance;

    [System.Serializable]
    public class SpawnPointData
    {
        public Transform spawnPointTransform;   // Le Transform du SpawnPoint
        public Transform[] waypoints;           // Les waypoints associés à ce spawn
    }

    public SpawnPointData[] spawnPointsData;     // Liste de tous les chemins

    void Awake()
    {
        instance = this;
    }
}

