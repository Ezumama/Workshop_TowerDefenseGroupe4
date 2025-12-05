using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIText : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _goldCountTxt;
    [SerializeField] private TextMeshProUGUI _redBlueprintCountTxt;
    [SerializeField] private TextMeshProUGUI _greenBlueprintCountTxt;
    [SerializeField] private TextMeshProUGUI _yellowBlueprintCountTxt;
    [SerializeField] private TextMeshProUGUI _energyCountTxt;

    void Update()
    {
        _goldCountTxt.text = GameManager.Instance.CurrentMoneyAmount.ToString();
        _redBlueprintCountTxt.text = GameManager.Instance.CurrentRedBlueprintAmount.ToString();
        _greenBlueprintCountTxt.text = GameManager.Instance.CurrentGreenBlueprintAmount.ToString();
        _yellowBlueprintCountTxt.text = GameManager.Instance.CurrentYellowBlueprintAmount.ToString();
        //_energyCountTxt.text = GameManager.Instance.CurrentEnergyAmount.ToString();
    }
}
