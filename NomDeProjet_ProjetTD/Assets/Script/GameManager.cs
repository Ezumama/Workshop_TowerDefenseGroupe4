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

    public int CurrentEnergyAmount;
    public int BaseEnergyAmount;

    private void Start()
    {
        CurrentMoneyAmount = BaseMoneyAmount;
        CurrentRedBlueprintAmount = BaseRedBlueprintAmount;
        CurrentYellowBlueprintAmount = BaseYellowBlueprintAmount;
        CurrentGreenBlueprintAmount = BaseGreenBlueprintAmount;
        CurrentEnergyAmount = BaseEnergyAmount;
    }

    private int ClampToZero(int value)
    {
        return Mathf.Max(value, 0);
    }


    #region money

    public void GainMoney(int amount)
    {
        CurrentMoneyAmount += amount;
    }

    public void LoseMoney(int amount)
    {
        CurrentMoneyAmount = ClampToZero(CurrentMoneyAmount - amount);
    }

    #endregion

    #region blueprints
    public void GainRedBlueprint(int amount)
    {
        CurrentRedBlueprintAmount += amount;
    }

    public void LoseRedBlueprint(int amount)
    {
        CurrentRedBlueprintAmount = ClampToZero(CurrentRedBlueprintAmount - amount);
    }

    public void GainYellowBlueprint(int amount)
    {
        CurrentYellowBlueprintAmount += amount;
    }

    public void LoseYellowBlueprint(int amount)
    {
        CurrentYellowBlueprintAmount = ClampToZero(CurrentYellowBlueprintAmount - amount);
    }

    public void GainGreenBlueprint(int amount)
    {
        CurrentGreenBlueprintAmount += amount;
    }

    public void LoseGreenBlueprint(int amount)
    {
        CurrentGreenBlueprintAmount = ClampToZero(CurrentGreenBlueprintAmount - amount);
    }
    #endregion

    #region energy

    public void GainEnergy(int amount)
    {
        CurrentEnergyAmount += amount;
    }

    public void LoseEnergy(int amount)
    {
        CurrentEnergyAmount = ClampToZero(CurrentEnergyAmount - amount);
    }

    #endregion
}
