using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private Slider _volume;
    [SerializeField] private AudioSource _gameMusic;

    [SerializeField] private TextMeshProUGUI _textUIFullscreenWindowed;

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

    #region option panel
    public void BackButton()
    {
        optionPanel.SetActive(false);
    }

    public void FullscreenOnOff()
    {
        if (Screen.fullScreen == true)
        {
            _textUIFullscreenWindowed.SetText("Windowed Mode");
            Screen.fullScreen = false;
        }
        else
        {
            _textUIFullscreenWindowed.SetText("Fullscreen Mode");
            Screen.fullScreen = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        _gameMusic.volume = _volume.value;
    }
    #endregion
}
