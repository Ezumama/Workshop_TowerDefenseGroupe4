using System.Collections;
using TMPro;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _towers;
    [SerializeField] private GameObject towerChoicePanelPrefab;

    private int _gatlingCost;
    private int _teslaCost;
    private int _groundCost;

    private int _gatlingEnergyCost;
    private int _teslaEnergyCost;
    private int _groundEnergyCost;

    [Header("Tower Cost UI Text")]
    [SerializeField] private TextMeshProUGUI _gatlingCostText;
    [SerializeField] private TextMeshProUGUI _teslaCostText;
    [SerializeField] private TextMeshProUGUI _groundCostText;
    [SerializeField] private TextMeshProUGUI _gatlingEnergyCostText;
    [SerializeField] private TextMeshProUGUI _teslaEnergyCostText;
    [SerializeField] private TextMeshProUGUI _groundEnergyCostText;

    [Header("Tower Animation")]
    [SerializeField] private Animator _towerAnimator;
    [SerializeField] private float _openAnimationDuration = 1.0f; // Time to wait before spawning tower after opening animation

    private TowerChoiceUI _choiceUIScript;
    private GameObject _towerChoicePanel;
    private Camera _camera;
    private bool _isBuilding = false;

    private void Start()
    {
        // Get TowerChoiceUI script from UI Panel and assign Spawner to this tower spawner
        _towerChoicePanel = Instantiate(towerChoicePanelPrefab, transform);
        _towerChoicePanel.SetActive(false);

        _choiceUIScript = _towerChoicePanel.GetComponentInChildren<TowerChoiceUI>();
        _choiceUIScript.SetSpawner(this);
        _camera = Camera.main;

        // Set cost
        _gatlingCost = GameManager.Instance.GatlingCost;
        _teslaCost = GameManager.Instance.TeslaCost;
        _groundCost = GameManager.Instance.GroundCost;
        _gatlingEnergyCost = GameManager.Instance.GatlingEnergyCost;
        _teslaEnergyCost = GameManager.Instance.TeslaEnergyCost;
        _groundEnergyCost = GameManager.Instance.GroundEnergyCost;
    }

    public void SpawnTower(int index)
    {
        // Spawn specific tower and destroy spawner
        if (!_isBuilding)
        {
            StartCoroutine(BuildSequence(index));
        }
    }

    private IEnumerator BuildSequence(int index)
    {
        _isBuilding = true;
        
        // Hide the UI
        _towerChoicePanel.SetActive(false);

        // Play the Opening Animation
        _towerAnimator.SetTrigger("Open");

        // Set The status to open
        _towerAnimator.SetBool("IsOpen", true);

        // Wait for the animation to finish
        yield return new WaitForSeconds(_openAnimationDuration);

        // Spawn specific tower
        Instantiate(_towers[index], transform.position, Quaternion.identity);
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