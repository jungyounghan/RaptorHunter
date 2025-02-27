using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// °ÔÀÓ UI¸¦ ÃÑ°ýÇÏ´Â Å¬·¡½º
/// </summary>
public sealed class State : MonoBehaviour
{
    [Serializable]
    public struct Gage
    {
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private TMP_Text text;

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
    private TMP_Text _damageText;
    [SerializeField]
    private TMP_Text _waveText;
    [SerializeField]
    private TMP_Text _timerText;
    [SerializeField]
    private TMP_Text _killText;
    [SerializeField]
    private TMP_Text _noticeText;
    [SerializeField]
    private PausePanel _pausePanel;

    public void SetLife(uint current, uint max)
    {
        _lifeGage.Set(current, max);
    }

    public void SetStamina(float current, float max)
    {
        _staminaGage.Set(current, max);
    }

    public void SetDamage(uint value)
    {
        if(_damageText != null)
        {
            _damageText.text = "Damage: " + value;
        }
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

    public void ShowPause(bool playing)
    {
        if(_pausePanel != null)
        {
            if(playing == true)
            {
                if(_pausePanel.gameObject.activeInHierarchy == true)
                {
                    _pausePanel.Close();
                }
                else
                {
                    _pausePanel.Open(true);
                }
            }
            else
            {
                _pausePanel.Open(false);
            }
        }
    }
}