using UnityEngine;

public class TowerUpgradeUI : MonoBehaviour
{
    private TowerUpgrade _towerUpgradeScript;

    private void Awake()
    {
        Debug.Log("TowerUpgradeUI Awakened: " + gameObject.name);
    }

    public void SetUpgrade(TowerUpgrade _upgradeScript)
    {
        Debug.Log("SetUpgrade CALLED on: " + gameObject.name);
        _towerUpgradeScript = _upgradeScript;
    }

    public void UpgradeLvl2Clicked()
    {
        _towerUpgradeScript.UpgradeTowerLevel2();
    }

    public void UpgradeLvl3Clicked()
    {
        _towerUpgradeScript.UpgradeTowerLevel3();
    }
}
