using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ����� ��ȯ�� �� �ε� ����� �˷��ִ� Ŭ����
/// </summary>
public sealed class LoadingPanel : Panel
{
    [Header("�ε��� �̹���"), SerializeField]
    private Image _loadingImage = null;

    [Header("�ε��� �ؽ�Ʈ"), SerializeField]
    private TMP_Text _loadingText = null;

    private AsyncOperation _asyncOperation = null;

    private static readonly string PercentText = "%";

    private enum DecimalPrecision: byte
    {
        None,      // �Ҽ��� ����
        One,       // �Ҽ��� ù° �ڸ�
        Two        // �Ҽ��� ��° �ڸ�
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