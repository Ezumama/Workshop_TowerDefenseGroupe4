using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region instance
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    [Header("Ressources")]
    public int CurrentMoneyAmount;
    public int BaseMoneyAmount;

    private void Start()
    {
        CurrentMoneyAmount = BaseMoneyAmount;
    }

    public void GainMoney(int amount)
    {
        CurrentMoneyAmount += amount;
    }
    
    public void LoseMoney(int amount)
    {
        CurrentMoneyAmount -= amount;
    }
}
