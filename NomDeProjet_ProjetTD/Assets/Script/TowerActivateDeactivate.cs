using UnityEngine;

public class TowerActivateDeactivate : MonoBehaviour
{
    [Header("Tower Emissive Material")]
    [SerializeField] private Material _towerMaterial;

    private void Start()
    {
        

       
    }

    private void DeactivateTower()
    {
        Color current = _towerMaterial.GetColor("_EmissionColor");
        // Change intensity
        float newIntensity = 0f;
        Color baseColor = current / current.maxColorComponent;

        _towerMaterial.SetColor("_EmissionColor", baseColor * newIntensity);
        _towerMaterial.EnableKeyword("_EMISSION");
    }

    private void ActivateTower()
    {
        Color current = _towerMaterial.GetColor("_EmissionColor");
        // Change intensity
        float newIntensity = 100f;
        Color baseColor = current / current.maxColorComponent;
        _towerMaterial.SetColor("_EmissionColor", baseColor * newIntensity);
        _towerMaterial.EnableKeyword("_EMISSION");
    }
}
