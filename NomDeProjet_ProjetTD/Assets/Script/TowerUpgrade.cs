using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    [SerializeField] private GameObject _towerLevel1;
    [SerializeField] private GameObject _towerLevel2;
    [SerializeField] private GameObject _towerLevel3;
    [SerializeField] private GameObject _towerChoicePanelPrefabLvl2;
    [SerializeField] private GameObject _towerChoicePanelPrefabLvl3;



    [Header("Upgrade Cost")]
    [SerializeField] private int _blueprintCostLvl2;
    [SerializeField] private int _blueprintCostLvl3;

    [Header("Upgrade Status")]
    [SerializeField] private bool _upgradedToLevel2;
    [SerializeField] private bool _upgradedToLevel3;

    [Header ("FX")]
    [SerializeField] private GameObject _upgradeFXLvl2;
    [SerializeField] private GameObject _upgradeFXLvl3;

    private GameObject _towerUpgradePanelLvl2;
    private GameObject _towerUpgradePanelLvl3;
    private TowerUpgradeUI _towerUpgradeUIScript;
    private Camera _camera;
    private GameObject _currentTower;

    private void Start()
    {
        _currentTower = this.gameObject;

        _towerUpgradePanelLvl2 = Instantiate(_towerChoicePanelPrefabLvl2, transform);
        _towerUpgradePanelLvl2.SetActive(false);
        _towerUpgradePanelLvl3 = Instantiate(_towerChoicePanelPrefabLvl3, transform);
        _towerUpgradePanelLvl3.SetActive(false);

        _towerUpgradeUIScript = _towerUpgradePanelLvl2.GetComponentInChildren<TowerUpgradeUI>();
        _towerUpgradeUIScript.SetUpgrade(this);

        _towerUpgradeUIScript = _towerUpgradePanelLvl3.GetComponentInChildren<TowerUpgradeUI>();
        _towerUpgradeUIScript.SetUpgrade(this);

        _camera = Camera.main;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnClick();
        }
    }

    // UI BUTTON VERSION
    public void UpgradeTowerLevel2()
    {
        ReplaceLvl2();
        GameManager.Instance.LoseRedBlueprint(_blueprintCostLvl2);
    }

    void ReplaceLvl2()
    {
        // Saving Level 1 position and rotation
        Vector3 pos = _currentTower.transform.position;
        Quaternion rot = _currentTower.transform.rotation;

        if (_upgradedToLevel2 == false)
        {
            // Destroy level 1 prefab, and spawn level 2 (upgrade)
            Destroy(_currentTower);
            _currentTower = Instantiate(_towerLevel2, pos, rot);
            Instantiate(_upgradeFXLvl2, pos, rot);

            _upgradedToLevel2 = true;
        }
    }

    public void UpgradeTowerLevel3()
    {
        ReplaceLvl3();
        GameManager.Instance.LoseRedBlueprint(_blueprintCostLvl3);
    }

    void ReplaceLvl3()
    {
        Vector3 pos1 = _currentTower.transform.position;
        Quaternion rot1 = _currentTower.transform.rotation;


        if (_upgradedToLevel3 == false)
        {
            Destroy(_currentTower);
            _currentTower = Instantiate(_towerLevel3, pos1, rot1);
            Instantiate(_upgradeFXLvl3, pos1, rot1);


            _upgradedToLevel3 = true;
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

        // Did we click this tower?
        TowerUpgrade towerHit = hit.collider.GetComponentInParent<TowerUpgrade>();

        // If we clicked something else, then close panel
        if (towerHit != this)
        {
            if (_towerUpgradePanelLvl2.activeSelf)
            {
                _towerUpgradePanelLvl2.SetActive(false);
            }

            return;
        }

        // If we clicked THIS tower, then open the panel
        if (_upgradedToLevel2 == false)
        {
            _towerUpgradePanelLvl2.SetActive(true);
        }

        else if (_upgradedToLevel2 == true && _upgradedToLevel3 == false)
        {
            _towerUpgradePanelLvl3.SetActive(true);
        }
    }
}