using UnityEngine;
using UnityEngine.Rendering;

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
    public int CurrentRedBlueprintAmount;
    public int CurrentYellowBlueprintAmount;
    public int CurrentGreenBlueprintAmount;
    public int BaseRedBlueprintAmount;
    public int BaseYellowBlueprintAmount;
    public int BaseGreenBlueprintAmount;

    private void Start()
    {
        CurrentMoneyAmount = BaseMoneyAmount;
        CurrentRedBlueprintAmount = BaseRedBlueprintAmount;
        CurrentYellowBlueprintAmount = BaseYellowBlueprintAmount;
        CurrentGreenBlueprintAmount = BaseGreenBlueprintAmount;
    }

    #region money

    public void GainMoney(int amount)
    {
        CurrentMoneyAmount += amount;
    }

    public void LoseMoney(int amount)
    {
        CurrentMoneyAmount -= amount;
    }

    #endregion

    #region blueprints
    public void GainRedBlueprint(int amount)
    {
        CurrentRedBlueprintAmount += amount;
    }

    public void LoseRedBlueprint(int amount)
    {
        CurrentRedBlueprintAmount -= amount;
    }

    public void GainYellowBlueprint(int amount)
    {
        CurrentYellowBlueprintAmount += amount;
    }

    public void LoseYellowBlueprint(int amount)
    {
        CurrentYellowBlueprintAmount -= amount;
    }

    public void GainGreenBlueprint(int amount)
    {
        CurrentGreenBlueprintAmount += amount;
    }

    public void LoseGreenBlueprint(int amount)
    {
        CurrentGreenBlueprintAmount -= amount;
    }
    #endregion
}
