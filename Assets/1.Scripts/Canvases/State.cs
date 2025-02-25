using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// °ÔÀÓ UI¸¦ ÃÑ°ýÇÏ´Â Å¬·¡½º
/// </summary>
public class State : MonoBehaviour
{
    [Serializable]
    public struct Gage
    {
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private Text text;

        private static readonly string DivisionText = " / ";

        public void Set(uint current, uint max)
        {
            if(slider != null)
            {
                if (max > 0)
                {
                    slider.value = (float)current / max;
                }
                else
                {
                    slider.value = 1;
                }
            }
            if(text != null)
            {
                text.text = current + DivisionText + max;
            }
        }

        public void Set(float current, float max)
        {
            if(slider != null)
            {
                if (max != 0)
                {
                    slider.value = current / max;
                }
                else
                {
                    slider.value = 1;
                }
            }
            if (text != null)
            {
                text.text = Mathf.Floor(current) + DivisionText + Mathf.Floor(max);
            }
        }
    }

    [SerializeField]
    private Gage _lifeGage;
    [SerializeField]
    private Gage _staminaGage;
    [SerializeField]
    private Text _waveText;
    [SerializeField]
    private Text _timerText;
    [SerializeField]
    private Text _killText;
    [SerializeField]
    private Text _noticeText;
    [SerializeField]
    private GameObject _popupObject = null;
    [SerializeField]
    private GameObject _playingObject = null;
    [SerializeField]
    private GameObject _gameOverObject = null;
    [SerializeField]
    private AudioPanel _audioPanel = null;

    public void SetLife(uint current, uint max)
    {
        _lifeGage.Set(current, max);
    }

    public void SetStamina(float current, float max)
    {
        _staminaGage.Set(current, max);
    }

    public void SetWave(uint number)
    {
        if(_waveText != null)
        {
            _waveText.text = number + " Wave";
        }
    }

    public void SetTimer(float time)
    {
        if(_timerText != null)
        {
            if (time > 0)
            {
                _timerText.text = time.ToString("n2");
            }
            else
            {
                _timerText.text = "";
            }
        }
    }

    public void SetKill(uint count)
    {
        if(_killText != null)
        {
            _killText.text = count + " Kill";
        }
    }

    public void SetNotice(string text = "")
    {
        if(_noticeText != null)
        {
            _noticeText.text = text;
        }
    }

    public void ShowPopup(bool playing)
    {
        if (_popupObject != null)
        {
            if(playing == true)
            {
                if(_popupObject.activeInHierarchy == true)
                {
                    Time.timeScale = 1;
                    _popupObject.SetActive(false);
                }
                else if(_playingObject != null)
                {
                    Time.timeScale = 0;
                    _popupObject.SetActive(true);
                    _playingObject.SetActive(true);
                    if(_audioPanel != null)
                    {
                        _audioPanel.SetActive(false);
                    }
                }
            }
            else
            {
                Time.timeScale = 0;
                _popupObject.SetActive(true);
            }
        }
    }

    public void ShowAudioPanel()
    {
        if(_playingObject != null && _playingObject.activeInHierarchy == true && _audioPanel != null)
        {
            _playingObject.SetActive(false);
            _audioPanel.SetActive(true);
        }
    }

    public void ReturnPrevious()
    {
        if (_popupObject != null && _popupObject.activeInHierarchy == true)
        {
            if (_audioPanel != null && _audioPanel.isActiveAndEnabled)
            {
                _audioPanel.SetActive(false);
                if (_playingObject != null)
                {
                    _playingObject.SetActive(true);
                }
            }
            else if (_playingObject != null && _playingObject.activeInHierarchy == true)
            {
                Time.timeScale = 1;
                _popupObject.SetActive(false);
            }
        }
    }
}