using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    [SerializeField] private GameObject _towerLevel1;
    [SerializeField] private GameObject _towerLevel2;
    [SerializeField] private GameObject _towerLevel3;
    [SerializeField] private GameObject _towerChoicePanelPrefab;
    [SerializeField] private bool _upgraded = false;

    [Header("Upgrade Cost")]
    [SerializeField] private int _blueprintCost;

    private GameObject _towerUpgradePanel;
    private TowerUpgradeUI _towerUpgradeUIScript;
    private Camera _camera;

    private void Start()
    {
        _towerUpgradePanel = Instantiate(_towerChoicePanelPrefab, transform);
        _towerUpgradePanel.SetActive(false);

        _towerUpgradeUIScript = _towerUpgradePanel.GetComponentInChildren<TowerUpgradeUI>();
        _towerUpgradeUIScript.SetUpgrade(this);
        _camera = Camera.main;
    }

    private void Update()
    {
        //// Debug : press T to upgrade tower. LATER ON : UI
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    Replace();
        //    GameManager.Instance.LoseBlueprint(1);
        //}

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnClick();
        }
    }

        // UI BUTTON VERSION
    public void UpgradeTowerLevel2()
    {
        Replace();
        Debug.Log("tuesuncaca");
        GameManager.Instance.LoseBlueprint(_blueprintCost);
    }

    void Replace()
    {
        // Saving Level 1 position and rotation
        Vector3 pos = _towerLevel1.transform.position;
        Quaternion rot = _towerLevel1.transform.rotation;

        if (_upgraded == false)
        {
            // Destroy level 1 prefab, and spawn level 2 (upgrade)
            Destroy(_towerLevel1);
            _towerLevel1 = Instantiate(_towerLevel2, pos, rot);
            _upgraded = true;
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
            if (_towerUpgradePanel.activeSelf)
            {
                _towerUpgradePanel.SetActive(false);
            }

            return;
        }

        // If we clicked THIS tower, then open panel
        _towerUpgradePanel.SetActive(true);
    }
}

