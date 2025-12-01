using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    [SerializeField] private GameObject _towerLevel1;
    [SerializeField] private GameObject _towerLevel2;
    [SerializeField] private GameObject _towerLevel3;
    [SerializeField] private GameObject _towerChoicePanelPrefabLvl2;
    [SerializeField] private GameObject _towerChoicePanelPrefabLvl3;
    [SerializeField] private bool _upgradedToLevel2 = false;
    [SerializeField] private bool _upgradedToLevel3 = false;

    [Header("Upgrade Cost")]
    [SerializeField] private int _blueprintCostLvl2;
    [SerializeField] private int _blueprintCostLvl3;

    private GameObject _towerUpgradePanelLvl2;
    private GameObject _towerUpgradePanelLvl3;
    private TowerUpgradeUI _towerUpgradeUIScript;
    private Camera _camera;

    private void Start()
    {
        _towerUpgradePanelLvl2 = Instantiate(_towerChoicePanelPrefabLvl2, transform);
        _towerUpgradePanelLvl2.SetActive(false);

        _towerUpgradeUIScript = _towerUpgradePanelLvl2.GetComponentInChildren<TowerUpgradeUI>();
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
        GameManager.Instance.LoseBlueprint(_blueprintCostLvl2);
    }

    void ReplaceLvl2()
    {
        // Saving Level 1 position and rotation
        Vector3 pos = _towerLevel1.transform.position;
        Quaternion rot = _towerLevel1.transform.rotation;

        if (_upgradedToLevel2 == false)
        {
            // Destroy level 1 prefab, and spawn level 2 (upgrade)
            Destroy(_towerLevel1);
            _towerLevel1 = Instantiate(_towerLevel2, pos, rot);
            _upgradedToLevel2 = true;
        }
    }

    void ReplaceLvl3()
    {
        // Saving Level 2 position and rotation
        Vector3 pos = _towerLevel2.transform.position;
        Quaternion rot = _towerLevel2.transform.rotation;

        if (_upgradedToLevel3 == false)
        {
            // Destroy level 2 prefab, and spawn level 3 (upgrade)
            Destroy(_towerLevel2);
            _towerLevel2 = Instantiate(_towerLevel3, pos, rot);
            _upgradedToLevel2 = false;
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

            if (_towerUpgradePanelLvl3.activeSelf)
            {
                _towerUpgradePanelLvl3.SetActive(false);
            }

            return;
        }

        // If we clicked THIS tower, then check what level it is at
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

