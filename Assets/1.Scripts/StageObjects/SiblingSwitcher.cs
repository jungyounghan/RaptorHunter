using UnityEngine;

public class SiblingSwitcher : MonoBehaviour
{
#if UNITY_EDITOR
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform {
        get
        {
            if (_hasTransform == false)
            {
                _hasTransform = true;
                _transform = transform;
            }
            return _transform;
        }
    }

    [Header("하이라키 순서를 의미하는 인덱스"),SerializeField, Range(0, int.MaxValue)]
    private int _index;

    private void OnValidate()
    {
        getTransform.SetSiblingIndex(_index);
    }
#endif
}