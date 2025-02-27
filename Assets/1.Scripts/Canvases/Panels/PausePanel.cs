using System;
using UnityEngine;

/// <summary>
/// 일시정지를 담당하는 패널 클래스
/// </summary>
public sealed class PausePanel : Panel
{
    [SerializeField]
    private GameObject _aliveObject = null;
    [SerializeField]
    private GameObject _deadObject = null;
    [SerializeField]
    private AudioPanel _audioPanel = null;
    [Header("로딩 프리팹"), SerializeField]
    private LoadingPanel _loadingPanel = null;

    private bool _playing = true;

    public override void Close()
    {
        if (_playing == true)
        {
            if(_audioPanel != null && _audioPanel.gameObject.activeInHierarchy == true)
            {
                _audioPanel.Close();
                if(_aliveObject != null)
                {
                    _aliveObject.SetActive(true);
                }
            }
            else
            {
                base.Close();
                Time.timeScale = 1;
            }
        }
    }

    public override void Open()
    {
        Time.timeScale = 0;
        base.Open();
    }

    public void Open(bool playing)
    {
        if (_playing == true)
        {
            Open();
            _playing = playing;
            if(_aliveObject != null)
            {
                _aliveObject.SetActive(_playing);
            }
            if(_deadObject != null)
            {
                _deadObject.SetActive(!_playing);
            }
            _audioPanel?.Close();
        }
    }

    public void OpenAudioPanel()
    {
        if(_aliveObject != null)
        {
            _aliveObject.SetActive(false);
        }
        _audioPanel?.Open();
    }

    public void LoadScene(string scene)
    {
        if (_loadingPanel != null)
        {
            LoadingPanel loadingPanel = Instantiate(_loadingPanel, getRectTransform);
            loadingPanel.Open(scene);
        }
    }
}