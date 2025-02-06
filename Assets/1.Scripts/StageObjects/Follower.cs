using UnityEngine;

/// <summary>
/// 대상을 따라가는 클래스
/// </summary>
[DisallowMultipleComponent]
public class Follower : MonoBehaviour
{
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

    [Header("추적할 대상의 트랜스폼"), SerializeField]
    private Transform _target = null;

    [Header("멀어질 거리"), SerializeField, Range(2.0f, 20)]
    private float _distance = 3.5f;

    [Header("높이"), SerializeField, Range(0, 10)]
    private float height = 3.0f;

    [Header("오프셋"), SerializeField, Range(0.5f, 15.0f)]
    private float _offset = 2.0f;

    [Header("보간 속도"), SerializeField, Range(float.Epsilon, 20)]
    private float _speed = 1.0f;

    private void LateUpdate()
    {
        if (_target != null)
        {
            float deltaTime = Time.deltaTime;
            Vector3 pivot = getTransform.position;
            Vector3 focus = _target.position;
            getTransform.position = Vector3.Lerp(pivot, focus + (-_target.forward * _distance) + (Vector3.up * height), _speed * deltaTime);
            getTransform.rotation = Quaternion.Slerp(getTransform.rotation, Quaternion.LookRotation((focus + _target.up * _offset) - pivot), _speed * deltaTime);
        }
    }
}