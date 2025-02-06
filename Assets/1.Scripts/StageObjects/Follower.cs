using UnityEngine;

/// <summary>
/// ����� ���󰡴� Ŭ����
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

    [Header("������ ����� Ʈ������"), SerializeField]
    private Transform _target = null;

    [Header("�־��� �Ÿ�"), SerializeField, Range(2.0f, 20)]
    private float _distance = 10.0f;

    [Header("����"), SerializeField, Range(0, 10)]
    private float height = 2.0f;

    [Header("������"), SerializeField, Range(0.5f, 15.0f)]
    private float _offset = 2.0f;

    [Header("���� �ӵ�"), SerializeField, Range(float.Epsilon, 20)]
    private float _speed = 1;

    private void LateUpdate()
    {
        if (_target != null)
        {
            getTransform.position = Vector3.Lerp(getTransform.position, _target.position + (-_target.forward * _distance) + (Vector3.up * height), _speed * Time.deltaTime);
            //getTransform.LookAt(_target.position + _target.up * _offset);
            Quaternion targetRotation = Quaternion.LookRotation((_target.position + _target.up * _offset) - getTransform.position);
            getTransform.rotation = Quaternion.Slerp(getTransform.rotation, targetRotation, _speed * Time.deltaTime);
        }
    }
}