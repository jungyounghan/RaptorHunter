using System;
using UnityEngine;
using UnityEngine.UI;

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

    }

    public void SetTimer(float time)
    {
        if(_timerText != null)
        {
            //소수점 2번째 자리까지 하기
            _timerText.text = Mathf.Floor(time).ToString();
        }
    }

    public void SetKill(uint count)
    {
        if(_killText != null)
        {
            _killText.text = count + " Kill";
        }
    }

}