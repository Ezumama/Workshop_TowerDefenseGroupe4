using UnityEngine;

public class WaypointNode : MonoBehaviour
{
    public Transform[] nextPaths;

    void OnValidate()
    {
        // Remplir automatiquement les chemins enfants
        nextPaths = new Transform[transform.childCount];
        for (int i = 0; i < nextPaths.Length; i++)
            nextPaths[i] = transform.GetChild(i);
    }
}

