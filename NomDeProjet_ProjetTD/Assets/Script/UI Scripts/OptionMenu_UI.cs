using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu_UI : MonoBehaviour
{
    private GameObject optionPanel;
    [SerializeField] private Slider _volume;
    [SerializeField] private AudioSource _gameMusic;
    [SerializeField] private Toggle _fullscreenTog, _vsyncTog;

    [SerializeField] private List<ResItem> resolutions = new List<ResItem>();

    [SerializeField] private TextMeshProUGUI _resolutionLabel;
    private int _selectedResolution;

    private void Start()
    {
        optionPanel = gameObject;
        _fullscreenTog.isOn = Screen.fullScreen;

        if(QualitySettings.vSyncCount == 0)
        {
            _vsyncTog.isOn = false;
        }
        else
        {
            _vsyncTog.isOn= true;
        }

        bool _foundRes = false;
        for(int i = 0; i < resolutions.Count; i++)
        {
            if(Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                _foundRes = true;
                _selectedResolution = i;
                UpdateResLabel();
            }
        }

        if(!_foundRes)
        {
            ResItem newRes = new ResItem();
            newRes.horizontal = Screen.width;
            newRes.vertical = Screen.height;

            resolutions.Add(newRes);
            _selectedResolution = resolutions.Count - 1;

            UpdateResLabel();
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

    #region Resolution
    public void ResLeft()
    {
        _selectedResolution--;
        if (_selectedResolution < 0)
        {
            _selectedResolution = 0;
        }
        UpdateResLabel();
    }

    public void ResRight()
    {
        _selectedResolution++;
        if(_selectedResolution > resolutions.Count - 1)
        {
            _selectedResolution = resolutions.Count - 1;
        }
        UpdateResLabel();
    }

    public void UpdateResLabel()
    {
        _resolutionLabel.text = resolutions[_selectedResolution].horizontal.ToString() + " x " + resolutions[_selectedResolution].vertical.ToString();
    }
    #endregion

    public void ApplyGraphics()
    {

        if(_vsyncTog.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        Screen.SetResolution(resolutions[_selectedResolution].horizontal, resolutions[_selectedResolution].vertical, _fullscreenTog.isOn);
    }

    [System.Serializable]
    public class ResItem
    {
        public int horizontal, vertical;
    }
}
