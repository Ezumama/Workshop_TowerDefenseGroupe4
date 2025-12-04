using UnityEngine;

public class TowerActivateDeactivate : MonoBehaviour
{
    [Header("DEBUG")]
    [SerializeField] private bool _isActivated = true;

    private bool _previousState;

    private TowerSpawner _towerSpawner;
    private int _tripleMelCost;
    private int _bigBettyCost;
    private int _simpleLizaCost;

    [Header("Select the tower")]
    [SerializeField] private bool _isTripleMel;
    [SerializeField] private bool _isBigBetty;
    [SerializeField] private bool _isSimpleLiza;

    private void Start()
    {
        _previousState = _isActivated;

        _towerSpawner = GetComponent<TowerSpawner>();

        _tripleMelCost = GameManager.Instance.GatlingEnergyCost;
        _bigBettyCost = GameManager.Instance.TeslaEnergyCost;
        _simpleLizaCost = GameManager.Instance.GroundEnergyCost;
    }

    private void Update()
    {
        // Only run when the value changes
        if (_isActivated != _previousState)
        {
            if (_isActivated)
            {
                Debug.Log("Activating Tower (ONCE)");
                ActivateTower();
            }
            else
            {
                Debug.Log("Deactivating Tower (ONCE)");
                DeactivateTower();
            }

            // Store new state
            _previousState = _isActivated;
        }
    }

    private void DeactivateTower()
    {
        if (_isTripleMel)
            GameManager.Instance.GainEnergy(_tripleMelCost);

        else if (_isBigBetty)
            GameManager.Instance.GainEnergy(_bigBettyCost);

        else if (_isSimpleLiza)
            GameManager.Instance.GainEnergy(_simpleLizaCost);
    }

    private void ActivateTower()
    {
        if (_isTripleMel)
            GameManager.Instance.LoseEnergy(_tripleMelCost);

        else if (_isBigBetty)
            GameManager.Instance.LoseEnergy(_bigBettyCost);

        else if (_isSimpleLiza)
            GameManager.Instance.LoseEnergy(_simpleLizaCost);
    }
}
