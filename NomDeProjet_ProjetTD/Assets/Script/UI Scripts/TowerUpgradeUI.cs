using UnityEngine;

public class TowerUpgradeUI : MonoBehaviour
{
    private TowerSpawner _towerUpgradeScript;

    public void SetUpgrade(TowerSpawner _upgradeScript)
    {
        Debug.Log("SetUpgrade CALLED on: " + gameObject.name);
        _towerUpgradeScript = _upgradeScript;
    }
    public void UpgradeLvl2Clicked()
    {
        _towerUpgradeScript.UpgradeTowerLevel2();
        gameObject.SetActive(false);
    }

    public void UpgradeLvl3Clicked()
    {
        _towerUpgradeScript.UpgradeTowerLevel3();
        gameObject.SetActive(false);
    }
}
