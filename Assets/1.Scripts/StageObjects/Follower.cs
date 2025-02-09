using UnityEngine;

/// <summary>
/// 대상을 따라가는 클래스
/// </summary>
[DisallowMultipleComponent]
public sealed class Follower : MonoBehaviour
{
    private const float MinValue = float.Epsilon;
    private const float MaxValue = 10;

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

    [SerializeField]
    private Transform _target = null;

    public Transform target {
        get
        {
            return _target;
        }
        set
        {
            _target = value;
        }
    }

    [Header("위치 오프셋"), SerializeField]
    private Vector3 _offset;

    public Vector3 offset {
        get
        {
            return _offset;
        }
        set
        {
            _offset = value;
        }
    }

    [Header("보간 속도"), SerializeField, Range(MinValue, MaxValue)]
    private float _speed = 1;

    public float speed {
        get
        {
            return _speed;
        }
        set
        {
            _speed = Mathf.Clamp(value, MinValue, MaxValue);
        }
    }

    [Header("시점 회전 유무"), SerializeField]
    private bool _rotation = false;

    public bool rotation {
        get
        {
            return _rotation;
        }
        set
        {
            _rotation = value;
        }
    }

#if UNITY_EDITOR

    [Header("축 자동화 유무"), SerializeField]
    private bool _axis = false;

    private void OnValidate()
    {
        if(_target != null)
        {
            switch (_rotation)
            {
                case true:
                    if(_axis == false)
                    {
                        _offset = Quaternion.Inverse(_target.rotation) * (getTransform.position - _target.position);
                    }
                    else
                    {
                        getTransform.position = (_target.position + _target.right * _offset.x) + (_target.forward * _offset.z) + (_target.up * _offset.y);
                        getTransform.rotation = _target.rotation;
                    }
                    break;
                case false:
                    if(_axis == false)
                    {
                        _offset = getTransform.position - _target.position;
                    }
                    else
                    {
                        getTransform.position = _target.position + _offset;
                    }
                    break;
            }
        }
    }
#endif

    private void LateUpdate()
    {
        if (_target != null)
        {
            float deltaTime = Time.deltaTime;
            Vector3 start = getTransform.position;
            switch (_rotation)
            {
                case true:
                {
                    Vector3 end = (_target.position + _target.right * _offset.x) + (_target.forward * _offset.z) + (_target.up * _offset.y);
                    Vector3 position = Vector3.Lerp(end, start, _speed * deltaTime);
                    Quaternion rotation = Quaternion.Slerp(_target.rotation, getTransform.rotation, _speed * deltaTime);
                    getTransform.SetPositionAndRotation(position, rotation);
                }
                break;
                case false:
                {
                    Vector3 end = _target.position + _offset;
                    getTransform.position = Vector3.Lerp(end, start, _speed * deltaTime);
                }
                break;
            }
        }
    }
}