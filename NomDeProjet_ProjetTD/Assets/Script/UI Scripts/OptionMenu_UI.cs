using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu_UI : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private Slider _volume;
    [SerializeField] private AudioSource _gameMusic;
    [SerializeField] private Toggle _fullscreenTog, _vsyncTog;

    private List<ResItem> resolutions = new List<ResItem>();

    private void Start()
    {
        _fullscreenTog.isOn = Screen.fullScreen;

        if(QualitySettings.vSyncCount == 0)
        {
            _vsyncTog.isOn = false;
        }
        else
        {
            _vsyncTog.isOn= true;
        }
    }
    
    public void BackButton()
    {
        optionPanel.SetActive(false);
    }

    public void FullscreenOnOff()
    {
        if (Screen.fullScreen == true)
        {
            Screen.fullScreen = false;
        }
        else
        {
            Screen.fullScreen = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        _gameMusic.volume = _volume.value;
    }
    public void ApplyGraphics()
    {
        Screen.fullScreen = _fullscreenTog.isOn;

        if(_vsyncTog.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    [System.Serializable]
    public class ResItem
    {
        public int horizontal, vertical;
    }
}
