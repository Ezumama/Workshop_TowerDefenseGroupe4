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
        _towerSpawnerScript.GatlingChoice();
    }

    //public void TeslaClicked()
    //{
    //    _towerSpawnerScript.TeslaChoice();
    //}

    //public void AirClicked()
    //{
    //    _towerSpawnerScript.AirChoice();
    //}
}
