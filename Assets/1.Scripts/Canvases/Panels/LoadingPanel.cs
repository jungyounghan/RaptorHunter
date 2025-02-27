using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 장면을 전환할 때 로딩 경과를 알려주는 클래스
/// </summary>
public sealed class LoadingPanel : Panel
{
    [Header("로딩바 이미지"), SerializeField]
    private Image _loadingImage = null;

    [Header("로딩바 텍스트"), SerializeField]
    private TMP_Text _loadingText = null;

    private AsyncOperation _asyncOperation = null;

    private static readonly string PercentText = "%";

    private enum DecimalPrecision: byte
    {
        None,      // 소수점 버림
        One,       // 소수점 첫째 자리
        Two        // 소수점 둘째 자리
    }

    private DecimalPrecision _decimalPrecision = DecimalPrecision.None;

    private void OnEnable()
    {
        StartCoroutine(DoLoadScene());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Set(float progress)
    {
        if (_loadingImage != null)
        {
            _loadingImage.fillAmount = progress;
        }
        if (_loadingText != null)
        {
            _loadingText.text = progress.ToString("N" + _decimalPrecision) + PercentText;
        }
    }

    IEnumerator DoLoadScene()
    {
        yield return null;
        while (_asyncOperation != null && _asyncOperation.isDone == false)
        {
            if (_asyncOperation.progress >= 0.9f)
            {
                _asyncOperation.allowSceneActivation = true;
            }
            Set(_asyncOperation.progress);
            yield return null;
        }
        if (_asyncOperation != null && _asyncOperation.isDone == true)
        {
            Set(1);
        }
    }

    public void Open(string scene)
    {
        Open();
        Time.timeScale = 1;
        _asyncOperation = SceneManager.LoadSceneAsync(scene);
        _asyncOperation.allowSceneActivation = false;
        Set(0);
        StartCoroutine(DoLoadScene());
    }
}