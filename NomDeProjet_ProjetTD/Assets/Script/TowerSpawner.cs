using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _towers;
    [SerializeField] private GameObject towerChoicePanelPrefab;

    [SerializeField] private GraphicRaycaster _uiRaycaster;
    [SerializeField] private EventSystem _eventSystem;

    #region int costs
    private int _gatlingCost;
    private int _teslaCost;
    private int _groundCost;

    private int _gatlingEnergyCost;
    private int _teslaEnergyCost;
    private int _groundEnergyCost;
    #endregion

    #region tower cost UI texts
    [Header("Tower Cost UI Text")]
    [SerializeField] private TextMeshProUGUI _gatlingCostText;
    [SerializeField] private TextMeshProUGUI _teslaCostText;
    [SerializeField] private TextMeshProUGUI _groundCostText;
    [SerializeField] private TextMeshProUGUI _gatlingEnergyCostText;
    [SerializeField] private TextMeshProUGUI _teslaEnergyCostText;
    [SerializeField] private TextMeshProUGUI _groundEnergyCostText;
    #endregion

    #region tower animation
    [Header("Tower Animation")]
    [SerializeField] private Animator _towerAnimator;
    [SerializeField] private float _openAnimationDuration = 1.0f; // Time to wait before spawning tower after opening animation
    [SerializeField] private Transform _spawnStartPosition; // Where tower spawns from
    [SerializeField] private Transform _spawnEndPosition; // Where tower pops to
    [SerializeField] private float _spawnPopDuration = 0.5f; // Duration of the pop animation
    [Header("Ground / Vertical Animations")]
    [SerializeField] private float _sinkDepth = 1.0f; // How far the level1 sinks under ground
    [SerializeField] private float _sinkDuration = 0.6f; // How long it takes to sink
    [SerializeField] private float _riseDelay = 0.5f; // Delay before level2 rises
    [SerializeField] private float _riseDuration = 0.6f; // How long it takes to rise up
    #endregion

    #region prefabs
    private GameObject _towerLevel1;
    private GameObject _towerLevel2;
    private GameObject _towerLevel3;
    [SerializeField] private GameObject _towerChoicePanelPrefabLvl2;
    [SerializeField] private GameObject _towerChoicePanelPrefabLvl3;
    #endregion

    #region boolean to know which tower it is and it's upgrade level
    [Header("Boolean to know which tower it is")]
    private bool _isTripleMelTower;
    private bool _isBigBettyTower;
    private bool _isSimpleLizaTower;
    private int _levelUpgrade;
    #endregion

    #region upgrade cost
    [Header("Upgrade Cost")]
    [SerializeField] private int _blueprintCostLvl2;
    [SerializeField] private int _blueprintCostLvl3;
    #endregion

    #region FX
    //[Header("FX")]
    //[SerializeField] private GameObject _upgradeFXLvl2;
    //[SerializeField] private GameObject _upgradeFXLvl3;
    #endregion

    #region private variables
    private GameObject _towerUpgradePanelLvl2;
    private GameObject _towerUpgradePanelLvl3;
    private TowerUpgradeUI _towerUpgradeUIScript;
    private Camera _camera;
    private GameObject _currentTower;
    private TowerChoiceUI _choiceUIScript;
    private GameObject _towerChoicePanel;
    private bool _isBuilding = false;
    #endregion

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

        _towerUpgradePanelLvl2 = Instantiate(_towerChoicePanelPrefabLvl2, transform);
        _towerUpgradePanelLvl2.SetActive(false);
        _towerUpgradePanelLvl3 = Instantiate(_towerChoicePanelPrefabLvl3, transform);
        _towerUpgradePanelLvl3.SetActive(false);

        _towerUpgradeUIScript = _towerUpgradePanelLvl2.GetComponentInChildren<TowerUpgradeUI>();
        _towerUpgradeUIScript.SetUpgrade(this);

        _towerUpgradeUIScript = _towerUpgradePanelLvl3.GetComponentInChildren<TowerUpgradeUI>();
        _towerUpgradeUIScript.SetUpgrade(this);

        _levelUpgrade = 0;
    }

    public void SpawnTower(int index)
    {
        // Spawn specific tower and destroy spawner
        if (!_isBuilding)
        {
            StartCoroutine(BuildSequence(index));
        }
    }

    #region Tower Build Sequence
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

        // Spawn specific tower at start position
        Vector3 startPos = _spawnStartPosition != null ? _spawnStartPosition.position : transform.position;
        Vector3 endPos = _spawnEndPosition != null ? _spawnEndPosition.position : transform.position;
        
        _currentTower = Instantiate(_towers[index], startPos, Quaternion.identity, transform);
        _levelUpgrade = 1;

        // Pop animation: lerp from start to end position
        float elapsedTime = 0f;
        while (elapsedTime < _spawnPopDuration && _currentTower != null)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _spawnPopDuration;
            _currentTower.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // Ensure final position is exact
        if (_currentTower != null)
        {
            _currentTower.transform.position = endPos;
            // Start sinking animation so level1 appears to enter the ground
            StartCoroutine(SinkTower(_currentTower, endPos));
        }
    }
    #endregion
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // mouse hovering over a UI element?
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // if yes then don't do raycast.
                return;
            }

            // if not then proceed with click logic
            OnClick();
        }
    }

    #region tower choice
    public void GatlingChoice()
    {
        if (GameManager.Instance.CurrentMoneyAmount >= _gatlingCost && GameManager.Instance.CurrentEnergyAmount >= _gatlingEnergyCost)
        {
            SpawnTower(0);
            GameManager.Instance.LoseMoney(_gatlingCost);
            GameManager.Instance.LoseEnergy(_gatlingEnergyCost);
            _isTripleMelTower = true;
            _towerLevel2 = _towers[3];
            _towerLevel3 = _towers[4];
        }
        else if (GameManager.Instance.CurrentMoneyAmount < _gatlingCost)
        {
            Debug.Log("Not enough money to build Gatling Tower!");
            _gatlingCostText.color = Color.red;
        }
        else if (GameManager.Instance.CurrentEnergyAmount < _gatlingEnergyCost)
        {
            Debug.Log("Not enough energy to build Gatling Tower!");
            _gatlingEnergyCostText.color = Color.red;
        }

    }
    public void TeslaChoice()
    {
        if (GameManager.Instance.CurrentMoneyAmount >= _teslaCost && GameManager.Instance.CurrentEnergyAmount >= _teslaEnergyCost)
        {
            SpawnTower(1);
            GameManager.Instance.LoseMoney(_teslaCost);
            GameManager.Instance.LoseEnergy(_teslaEnergyCost);
            _isBigBettyTower = true;
            _towerLevel2 = _towers[5];
            _towerLevel3 = _towers[6];
        }
        else if (GameManager.Instance.CurrentMoneyAmount < _teslaCost)
        {
            Debug.Log("Not enough money to build Tesla Tower!");
            _teslaCostText.color = Color.red;
        }
        else if (GameManager.Instance.CurrentEnergyAmount < _teslaEnergyCost)
        {
            Debug.Log("Not enough energy to build Tesla Tower!");
            _teslaEnergyCostText.color = Color.red;
        }
    }

    //public void GroundChoice()
    //{
    //    if (GameManager.Instance.CurrentMoneyAmount >= _groundCost && GameManager.Instance.CurrentEnergyAmount >= _groundEnergyCost)
    //    {
    //        SpawnTower(2);
    //        GameManager.Instance.LoseMoney(_groundCost);
    //        GameManager.Instance.LoseEnergy(_groundEnergyCost);
    //        _isSimpleLizaTower = true;
    //        _towerLevel2 = _towers[7];
    //        _towerLevel3 = _towers[8];
    //    }
    //    else if (GameManager.Instance.CurrentMoneyAmount < _groundCost)
    //    {
    //        Debug.Log("Not enough money to build Ground Tower!");
    //        _groundCostText.color = Color.red;
    //    }
    //    else if (GameManager.Instance.CurrentEnergyAmount < _groundEnergyCost)
    //    {
    //        Debug.Log("Not enough energy to build Ground Tower!");
    //        _groundEnergyCostText.color = Color.red;
    //    }
    //}
    #endregion


    public void OnClick()
    {

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int mask = ~LayerMask.GetMask("Default");

        // What did we click?
        if (!Physics.Raycast(ray, out hit, 100, mask))
        {
            CloseAllPanels();
            return;
        }


        // Did we click this spawner?
        TowerSpawner spawnerHit = hit.collider.GetComponentInParent<TowerSpawner>();

        // Clicked something that is NOT this spawner
        if (spawnerHit != this)
        {
            CloseAllPanels();
            return;
        }

        // Did we click THIS spawner?
        if (spawnerHit == this)
        {
            // If we clicked THIS spawner, then see if there's already a tower
            if (_levelUpgrade == 0)
            {
                _towerChoicePanel.SetActive(true);
            }

            // if there's a tower, then spawn upgrade panel (lvl 2)
            else if (_levelUpgrade == 1)
            {
                _towerUpgradePanelLvl2.SetActive(true);
            }

            // if there's a tower, then spawn upgrade panel (lvl 3)
            else if (_levelUpgrade == 2)
            {
                _towerUpgradePanelLvl3.SetActive(true);
            }   

            return;
        }

    }

    private void CloseAllPanels()
    {
        _towerChoicePanel.SetActive(false);
        _towerUpgradePanelLvl2.SetActive(false);
        _towerUpgradePanelLvl3.SetActive(false);
    }

    // Coroutine: sink a tower downward so it appears to enter the ground
    private IEnumerator SinkTower(GameObject tower, Vector3 groundPos)
    {
        if (tower == null) yield break;

        Vector3 start = groundPos;
        Vector3 end = groundPos - Vector3.up * _sinkDepth;
        float elapsed = 0f;
        while (elapsed < _sinkDuration && tower != null)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _sinkDuration);
            tower.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        if (tower != null)
        {
            tower.transform.position = end;
        }
    }

    // Coroutine: wait then raise a tower from below to the target position
    private IEnumerator RiseTower(GameObject tower, Vector3 targetPos, float delay, float duration)
    {
        if (tower == null) yield break;

        // initial is below by _sinkDepth (caller should have placed it there already)
        Vector3 start = tower.transform.position;
        Vector3 end = targetPos;

        float waited = 0f;
        while (waited < delay)
        {
            waited += Time.deltaTime;
            yield return null;
        }

        float elapsed = 0f;
        while (elapsed < duration && tower != null)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            tower.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        if (tower != null)
        {
            tower.transform.position = end;
        }
    }

   #region Level 2 Upgrade
    public void UpgradeTowerLevel2()
    {
        ReplaceLvl2();
        if (_isTripleMelTower == true)
        {
            GameManager.Instance.LoseRedBlueprint(_blueprintCostLvl2);
        }

        else if (_isBigBettyTower == true)
        {
            GameManager.Instance.LoseGreenBlueprint(_blueprintCostLvl2);
        }
        else if (_isSimpleLizaTower == true)
        {
            GameManager.Instance.LoseYellowBlueprint(_blueprintCostLvl2);
        }
    }

    void ReplaceLvl2()
    {
        // Saving Level 1 position and rotation
        Vector3 pos = _currentTower.transform.position;
        Quaternion rot = _currentTower.transform.rotation;

        if (_levelUpgrade == 1)
        {
            if (_currentTower != null)
            {
                Destroy(_currentTower);
            }

            // Instantiate the level2 tower slightly below the ground and make it rise into place
            Vector3 belowPos = pos - Vector3.up * _sinkDepth;
            _currentTower = Instantiate(_towerLevel2, belowPos, rot, transform);

            // Start the rising coroutine which waits then moves tower up to original position
            StartCoroutine(RiseTower(_currentTower, pos, _riseDelay, _riseDuration));

            _levelUpgrade = 2;
        }
    }
    #endregion

   #region Level 3 Upgrade
    public void UpgradeTowerLevel3()
    {
        ReplaceLvl3();
        if (_isTripleMelTower == true)
        {
            GameManager.Instance.LoseRedBlueprint(_blueprintCostLvl3);
        }

        else if (_isBigBettyTower == true)
        {
            GameManager.Instance.LoseGreenBlueprint(_blueprintCostLvl3);
        }
        else if (_isSimpleLizaTower == true)
        {
            GameManager.Instance.LoseYellowBlueprint(_blueprintCostLvl3);
        }
    }

    void ReplaceLvl3()
    {
        Vector3 pos1 = _currentTower.transform.position;
        Quaternion rot1 = _currentTower.transform.rotation;


        if (_levelUpgrade == 2)
        {
            if (_currentTower != null)
            {
                Destroy(_currentTower);
            }
            
            _currentTower = Instantiate(_towerLevel3, pos1, rot1, transform);
            
            //Instantiate(_upgradeFXLvl3, pos1, rot1);


            _levelUpgrade = 3;
        }
    }
    #endregion
}