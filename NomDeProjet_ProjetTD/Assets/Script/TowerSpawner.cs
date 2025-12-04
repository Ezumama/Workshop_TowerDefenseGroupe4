using TMPro;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _towers;
    [SerializeField] private GameObject towerChoicePanelPrefab;

    [Header("Tower Cost Money")]
    [SerializeField] private int _gatlingCost;
    [SerializeField] private int _teslaCost;
    [SerializeField] private int _groundCost;

    [Header("Tower Cost Energy")]
    [SerializeField] private int _gatlingEnergyCost;
    [SerializeField] private int _teslaEnergyCost;
    [SerializeField] private int _groundEnergyCost;

    [Header("Tower Cost UI Text")]
    [SerializeField] private TextMeshProUGUI _gatlingCostText;
    [SerializeField] private TextMeshProUGUI _teslaCostText;
    [SerializeField] private TextMeshProUGUI _groundCostText;
    [SerializeField] private TextMeshProUGUI _gatlingEnergyCostText;
    [SerializeField] private TextMeshProUGUI _teslaEnergyCostText;
    [SerializeField] private TextMeshProUGUI _groundEnergyCostText;

    private TowerChoiceUI _choiceUIScript;
    private GameObject _towerChoicePanel;
    private Camera _camera;

    private void Start()
    {
        // Get TowerChoiceUI script from UI Panel and assign Spawner to this tower spawner
        _towerChoicePanel = Instantiate(towerChoicePanelPrefab, transform);
        _towerChoicePanel.SetActive(false);

        _choiceUIScript = _towerChoicePanel.GetComponentInChildren<TowerChoiceUI>();
        _choiceUIScript.SetSpawner(this);
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
        if (GameManager.Instance.CurrentMoneyAmount >= _gatlingCost && GameManager.Instance.CurrentEnergyAmount >= _gatlingEnergyCost)
        {
            SpawnTower(0);
            GameManager.Instance.LoseMoney(_gatlingCost);
            GameManager.Instance.LoseEnergy(_gatlingEnergyCost);
        }
        //else if (GameManager.Instance.CurrentMoneyAmount < _gatlingCost)
        //{
        //    Debug.Log("Not enough money to build Gatling Tower!");
        //    _gatlingCostText.color = Color.red;
        //}
        //else if (GameManager.Instance.CurrentEnergyAmount < _gatlingEnergyCost)
        //{
        //    Debug.Log("Not enough energy to build Gatling Tower!");
        //    _gatlingEnergyCostText.color = Color.red;
        // }

    }
    //public void TeslaChoice()
    //{
    //    if (GameManager.Instance.CurrentMoneyAmount >= __teslaCost && GameManager.Instance.CurrentEnergyAmount >= _teslaEnergyCost)
    //    {
    //      SpawnTower(1);
    //      GameManager.Instance.LoseMoney(_teslaCost);
    //      GameManager.Instance.LoseEnergy(_teslaEnergyCost);
    //    }
    //    else if (GameManager.Instance.CurrentMoneyAmount < _teslaCost)
    //    {
    //        Debug.Log("Not enough money to build Tesla Tower!");
    //        _teslaCostText.color = Color.red;
    //    }
    //    else if (GameManager.Instance.CurrentEnergyAmount < _teslaEnergyCost)
    //    {
    //        Debug.Log("Not enough energy to build Tesla Tower!");
    //       _teslaEnergyCostText.color = Color.red;
    //    }
    //}
    //public void GroundChoice()
    //{
    //    if (GameManager.Instance.CurrentMoneyAmount >= _groundCost && GameManager.Instance.CurrentEnergyAmount >= _groundEnergyCost)
    //    {
    //      SpawnTower(2);
    //      GameManager.Instance.LoseMoney(_groundCost);
    //      GameManager.Instance.LoseEnergy(_groundEnergyCost);
    //    }
    //    else if (GameManager.Instance.CurrentMoneyAmount < _groundCost)
    //    {
    //        Debug.Log("Not enough money to build Ground Tower!");
    //        _groundCostText.color = Color.red;
    //    }
    //    else if (GameManager.Instance.CurrentEnergyAmount < _groundEnergyCost)
    //    {
    //        Debug.Log("Not enough energy to build Ground Tower!");
    //       _groundEnergyCostText.color = Color.red;
    //    }
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

        int mask = ~LayerMask.GetMask("Default");

        // What did we click?
        if (!Physics.Raycast(ray, out hit, 100, mask))
        {
            return;
        }


        // Did we click this spawner?
        TowerSpawner spawnerHit = hit.collider.GetComponentInParent<TowerSpawner>();

        // If we clicked something else, then close panel
        if (spawnerHit != this)
        {
            if (_towerChoicePanel.activeSelf)
            {
                _towerChoicePanel.SetActive(false);
            }

            return;
        }

        // If we clicked THIS spawner, then open panel
        _towerChoicePanel.SetActive(true);
    }
}
