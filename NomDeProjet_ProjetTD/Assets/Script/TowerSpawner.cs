using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _towers;
    [SerializeField] private GameObject _towerChoicePanel;

    private TowerChoiceUI _choiceUIScript;

    private void Start()
    {
        // Get TowerChoiceUI script from UI Panel and assign Spawner to this tower spawner
        _choiceUIScript = _towerChoicePanel.GetComponent<TowerChoiceUI>();
        _choiceUIScript.SetSpawner(this);
    }

    public void SpawnTower(int index)
    {
        // Spawn specific tower and destroy spawner
        GameObject tower = Instantiate(_towers[index], transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void GatlingChoice()
    {
        SpawnTower(0);
    }
    //public void TeslaChoice()
    //{
    //    SpawnTower(1);
    //}
    //public void AirChoice()
    //{
    //    SpawnTower(2);
    //}
}
