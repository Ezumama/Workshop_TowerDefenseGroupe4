using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel;

    private void Start()
    {
        optionPanel.SetActive(false);
    }

    #region main menu panel
    public void StartGame()
    {
        SceneManager.LoadScene("Final_Level");
    }

    public void OptionMenu()
    {
        optionPanel.SetActive(true);
    }

    public void QuitGame()
    { 
        Application.Quit();
    }
    #endregion
}
