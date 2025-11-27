using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    [SerializeField] private GameObject _towerLevel1;
    [SerializeField] private GameObject _towerLevel2;
    [SerializeField] private bool _upgraded = false;

    private void Update()
    {
        // Debug : press T to upgrade tower. LATER ON : UI
        if (Input.GetKeyDown(KeyCode.T))
        {
            Replace();
        }

        //// UI BUTTON VERSION
        //void UpgradeTowerLevel2()
        //{
        //    Replace();
        //}

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
            }
        }
    }
}
