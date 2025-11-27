using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _towers;
    [SerializeField] private GameObject _towerChoicePanel;

    [Header("Tower Cost")]
    [SerializeField] private int _gatlingCost;
    [SerializeField] private int _teslaCost;
    [SerializeField] private int _airCost;

    private TowerChoiceUI _choiceUIScript;

    private Camera _camera;

    private void Start()
    {
        // Get TowerChoiceUI script from UI Panel and assign Spawner to this tower spawner
        //_choiceUIScript = _towerChoicePanel.GetComponent<TowerChoiceUI>();  
        //_choiceUIScript.SetSpawner(this);
        _camera = Camera.main;
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
        GameManager.Instance.LoseMoney(_gatlingCost);
    }
    //public void TeslaChoice()
    //{
    //    SpawnTower(1);
    //    GameManager.Instance.LoseMoney(_teslaCost);
    //}
    //public void AirChoice()
    //{
    //    SpawnTower(2);
    //    GameManager.Instance.LoseMoney(_airCost);
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnClick();
        }
    }

    public void OnClick()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.layer == LayerMask.NameToLayer("TowerSpawner"))
        {
            Debug.Log("Pressed Tower Spawner");
            // OPEN THE UI MENU
        }
    }
}
