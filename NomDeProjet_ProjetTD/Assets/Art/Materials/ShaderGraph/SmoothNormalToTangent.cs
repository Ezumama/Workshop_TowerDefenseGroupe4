using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// This script calculates "smoothed" normals (averaging normals of vertices at the same position)
// and stores them into the Mesh Tangents.
// This is commonly used for Outline Shaders or Toon Shading to prevent broken edges.

public class SmoothNormalBaker : MonoBehaviour
{
    [Tooltip("If true, the calculation runs automatically when the game starts.")]
    public bool runOnAwake = true;

    [Tooltip("If true, the mesh is cloned before modification to prevent changing the original asset.")]
    public bool cloneMesh = true;

    void Awake()
    {
        if (runOnAwake)
        {
            CalculateAndStoreSmoothNormals();
        }
    }

    // Context Menu allows you to run this from the Editor by right-clicking the component
    [ContextMenu("Bake Smooth Normals to Tangents")]
    public void CalculateAndStoreSmoothNormals()
    {
        // 1. Handle Static Meshes (MeshFilter)
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        foreach (var mf in meshFilters)
        {
            ProcessMeshFilter(mf);
        }

        // 2. Handle Rigged Meshes (SkinnedMeshRenderer)
        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var smr in skinnedMeshRenderers)
        {
            ProcessSkinnedMeshRenderer(smr);
        }

        int totalMeshes = meshFilters.Length + skinnedMeshRenderers.Length;
        if (totalMeshes > 0)
        {
            Debug.Log($"<color=cyan>Smooth Normals Baked</color> for {totalMeshes} meshes in hierarchy: {name}");
        }
        else
        {
            Debug.LogWarning("No MeshFilter or SkinnedMeshRenderer found in hierarchy.");
        }
    }

    private void ProcessMeshFilter(MeshFilter mf)
    {
        Mesh mesh = null;

        // Handle Mesh Instance vs Shared Mesh
        if (Application.isPlaying && cloneMesh)
        {
            // Accessing .mesh on a MeshFilter automatically instantiates a copy
            mesh = mf.mesh; 
        }
        else
        {
            // In editor (or if we want to modify the asset directly), use sharedMesh
            mesh = mf.sharedMesh;
        }

        if (mesh != null)
        {
            BakeSmoothNormalsForMesh(mesh);
        }
    }

    private void ProcessSkinnedMeshRenderer(SkinnedMeshRenderer smr)
    {
        Mesh mesh = smr.sharedMesh;

        if (mesh == null) return;

        // Handle Cloning for Skinned Meshes
        if (Application.isPlaying && cloneMesh)
        {
            // SkinnedMeshRenderer does not have a .mesh property that auto-clones.
            // We must manually instantiate it and assign it back.
            Mesh clonedMesh = Instantiate(mesh);
            smr.sharedMesh = clonedMesh;
            mesh = clonedMesh;
        }
        // If not cloning, we just modify the sharedMesh directly (modifies the asset)

        BakeSmoothNormalsForMesh(mesh);
    }

    private void BakeSmoothNormalsForMesh(Mesh mesh)
    {
        // 1. Get current data
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        
        // 2. Group vertices by position
        // We use a dictionary to accumulate normals for all vertices that occupy the exact same point in space.
        // This merges hard edges (where vertices are duplicated) into a single smooth direction.
        Dictionary<Vector3, Vector3> averageNormals = new Dictionary<Vector3, Vector3>();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (!averageNormals.ContainsKey(vertices[i]))
            {
                averageNormals.Add(vertices[i], normals[i]);
            }
            else
            {
                averageNormals[vertices[i]] += normals[i];
            }
        }

        // 3. Normalize the averaged vectors is handled implicitly during assignment below

        // 4. Assign the smooth normal to the Tangent channel
        Vector4[] tangents = new Vector4[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            // Retrieve the averaged normal for this position
            Vector3 smoothNormal = averageNormals[vertices[i]].normalized;

            // Store in Tangent. Tangents are Vector4. 
            // We store the normal in XYZ. W is usually 1 or -1 for binormal, we set to 0 here.
            tangents[i] = new Vector4(smoothNormal.x, smoothNormal.y, smoothNormal.z, 0f);
        }

        // 5. Apply back to mesh
        mesh.tangents = tangents;
    }
}