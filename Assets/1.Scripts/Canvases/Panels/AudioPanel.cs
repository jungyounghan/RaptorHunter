using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// ���� ȿ���� ������ �� �ִ� Ŭ����
/// </summary>
public sealed class AudioPanel : Panel
{
    [Header("����� �ͼ�"), SerializeField] 
    private AudioMixer _audioMixer;

    [Header("��ü ���� ����"), SerializeField] 
    private Slider _masterSlider;

    [Header("ȿ���� ����"), SerializeField]
    private Slider _effectSlider;

    [Header("����� ����"), SerializeField] 
    private Slider _backgroundSlider;

    private static readonly string MasterMixer = "Master";
    private static readonly string EffectMixer = "Effect";
    private static readonly string BackgroundMixer = "Background";

    private void Awake()
    {
        if (_masterSlider != null)
        {
            _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        if (_effectSlider != null)
        {
            _effectSlider.onValueChanged.AddListener(SetEffectVolume);
        }
        if (_backgroundSlider != null)
        {
            _backgroundSlider.onValueChanged.AddListener(SetBackgroundVolume);
        }
    }

    private void SetMasterVolume(float volume)
    {
        _audioMixer.SetFloat(MasterMixer, volume);
    }

    private void SetEffectVolume(float volume)
    {
        _audioMixer.SetFloat(EffectMixer, volume);
    }

    private void SetBackgroundVolume(float volume)
    {
        _audioMixer.SetFloat(BackgroundMixer, volume);
    }

    public override void Open()
    {
        base.Open();
        if(_audioMixer != null)
        {
            if (_audioMixer.GetFloat(MasterMixer, out float masterVolume) == true && _masterSlider != null)
            {
                _masterSlider.value = masterVolume;
            }
            if (_audioMixer.GetFloat(EffectMixer, out float effectVolume) == true && _effectSlider != null)
            {
                _effectSlider.value = effectVolume;
            }
            if (_audioMixer.GetFloat(BackgroundMixer, out float backgroundVolume) == true && _backgroundSlider != null)
            {
                _backgroundSlider.value = backgroundVolume;
            }
        }
    }
}