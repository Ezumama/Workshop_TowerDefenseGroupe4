using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel;

    private void Start()
    {
        optionPanel.SetActive(false);
    }

    void StartGame()
    {
        SceneManager.LoadScene("Final_Level");
    }

    void OptionMenu()
    {
        optionPanel.SetActive(true);
    }

    void BackButton()
    {
        optionPanel.SetActive(false);
    }

    void QuitGame()
    { 
        Application.Quit();
    }
}
