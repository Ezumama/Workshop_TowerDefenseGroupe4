using UnityEngine;

public class TowerUpgradeUI : MonoBehaviour
{
    private TowerUpgrade _towerUpgradeScript;

    public void SetUpgrade(TowerUpgrade _upgradeScript)
    {
        _towerUpgradeScript = _upgradeScript;
    }

    public void UpgradeClicked()
    {
        _towerUpgradeScript.UpgradeTowerLevel2();
    }
}
