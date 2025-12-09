using UnityEngine;

public class TowerChoiceUI : MonoBehaviour
{
    private TowerSpawner _towerSpawnerScript;

    public void SetSpawner(TowerSpawner _spawnerScript)
    {
        _towerSpawnerScript = _spawnerScript;
    }

    public void GatlingClicked()
    {
        Debug.Log("Spawner utilisé");
        _towerSpawnerScript.GatlingChoice();
    }

    public void TeslaClicked()
    {
        _towerSpawnerScript.TeslaChoice();
    }

    public void AirClicked()
    {
        Debug.Log("cheese");
        //_towerSpawnerScript.AirChoice();
    }
}
