using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 장면을 전환할 때 로딩바로 경과를 알려주는 클래스
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private Image _image = null;

    private AsyncOperation _asyncOperation = null;

    private void OnEnable()
    {
        StartCoroutine(DoLoadScene());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator DoLoadScene()
    {
        Time.timeScale = 1;
        yield return null;
        if (_asyncOperation != null)
        {
            while (_asyncOperation.isDone == false)
            {
                if (_asyncOperation.progress >= 0.9f)
                {
                    _asyncOperation.allowSceneActivation = true;
                }
                if (_image != null)
                {
                    _image.fillAmount = _asyncOperation.progress;
                }
                yield return null;
            }
            if(_image != null)
            {
                _image.fillAmount = 1;
            }
        }
    }

    public void Load(string scene)
    {
        _asyncOperation = SceneManager.LoadSceneAsync(scene);
        _asyncOperation.allowSceneActivation = false;
        if(_image != null)
        {
            _image.fillAmount = 0;
        }
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}