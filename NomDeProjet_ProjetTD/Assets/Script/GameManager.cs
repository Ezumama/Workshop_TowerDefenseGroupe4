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
    public int CurrentBlueprintAmount;
    public int BaseBlueprintAmount;

    private void Start()
    {
        CurrentMoneyAmount = BaseMoneyAmount;
        CurrentBlueprintAmount = BaseBlueprintAmount;
    }

    public void GainMoney(int amount)
    {
        CurrentMoneyAmount += amount;
    }
    
    public void LoseMoney(int amount)
    {
        CurrentMoneyAmount -= amount;
    }

    public void GainBlueprint(int amount)
    {
        CurrentBlueprintAmount += amount;
    }

    public void LoseBlueprint(int amount)
    {
        CurrentBlueprintAmount -= amount;
    }
}
