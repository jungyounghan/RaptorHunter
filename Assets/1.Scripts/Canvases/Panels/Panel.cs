using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Image UI를 가지고 있는 패널 클래스
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class Panel : MonoBehaviour
{
    private bool _hasRectTransform = false;

    private RectTransform _rectTransform = null;

    protected RectTransform getRectTransform {
        get
        {
            if (_hasRectTransform == false)
            {
                _hasRectTransform = TryGetComponent(out _rectTransform);
            }
            return _rectTransform;
        }
    }

    private bool _hasImage = false;

    private Image _image = null;

    protected Image getImage {
        get
        {
            if(_hasImage == false)
            {
                _hasImage = TryGetComponent(out _image);
            }
            return _image;
        }
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }
}