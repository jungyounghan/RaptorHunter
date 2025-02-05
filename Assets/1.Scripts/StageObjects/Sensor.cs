using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform
    {
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

    [Header("�þ� ������ ������"), SerializeField]
    public float _focusAngle;
    [Header("�þ� ������ ������"), SerializeField, Range(0, 360)]
    public float _rangeAngle;
    [Header("�þ� ���� ����"), SerializeField, Range(0, byte.MaxValue)]
    private float _updateTime = 0.4f;
    [Header("�þ� ������ �ݰ�"), SerializeField, Range(0, int.MaxValue)]
    public float _radius;
    [Header("�þ� ������ ������"), SerializeField]
    private Vector3 _offset;

    public LayerMask _layerMask;

    public LayerMask obstacleMask;

    public List<Collider> _targets = new List<Collider>();

    public event Action<IEnumerable<Transform>> targetAction = null;

#if UNITY_EDITOR
    [Header("���� ǥ�� ����"), SerializeField]
    private Color _rangeColor = Color.white;
    [Header("���� ǥ�� ����"), SerializeField]
    private Color _targetColor = Color.red;

    private void OnDrawGizmos()
    {
        float focusDegree = getTransform.eulerAngles.y + _focusAngle - 90;
        float angleHalf = _rangeAngle * 0.5f;
        Vector3 position = getTransform.position + _offset;
        Gizmos.color = _rangeColor;
        Gizmos.DrawWireSphere(position, _radius);
        Gizmos.DrawLine(new Vector3(position.x + _radius, position.y, position.z), new Vector3(position.x - _radius, position.y, position.z));
        Gizmos.DrawLine(new Vector3(position.x, position.y + _radius, position.z), new Vector3(position.x, position.y - _radius, position.z));
        Gizmos.DrawLine(new Vector3(position.x, position.y, position.z + _radius), new Vector3(position.x, position.y, position.z - _radius));
        Gizmos.DrawLine(position, position + new Vector3(Mathf.Cos((focusDegree + angleHalf) * Mathf.Deg2Rad), 0, Mathf.Sin((focusDegree + angleHalf) * Mathf.Deg2Rad)).normalized * _radius);
        Gizmos.DrawLine(position, position + new Vector3(Mathf.Cos((focusDegree - angleHalf) * Mathf.Deg2Rad), 0, Mathf.Sin((focusDegree - angleHalf) * Mathf.Deg2Rad)).normalized * _radius);
        Gizmos.color = _targetColor;
        foreach (Collider target in _targets)
        {
            Gizmos.DrawLine(position, target.transform.position);
        }
    }
#endif

    private void OnEnable()
    {
        StartCoroutine(DoDetectTargets());
        IEnumerator DoDetectTargets()
        {
            while (true)
            {
                _targets.Clear();
                float angleDegree = getTransform.eulerAngles.y + _focusAngle - 90;
                Vector3 center = getTransform.position + _offset;
                Vector3 currentAngle = new Vector3(Mathf.Cos(angleDegree * Mathf.Deg2Rad), 0, Mathf.Sin(angleDegree * Mathf.Deg2Rad));
                Collider[] colliders = Physics.OverlapSphere(center, _radius, _layerMask);
                foreach(Collider collider in colliders)
                {
                    Vector3 position = collider.transform.position;
                    Vector3 direction = (position - center).normalized;
                    // �÷��̾�� forward�� target�� �̷�� ���� ������ ���� �����
                    if (Vector3.Angle(/*transform.forward*/currentAngle, direction) < _rangeAngle * 0.5f)
                    {
                        float dstToTarget = Vector3.Distance(center, position);

                        // Ÿ������ ���� ����ĳ��Ʈ�� obstacleMask�� �ɸ��� ������ visibleTargets�� Add
                        if (!Physics.Raycast(center, direction, dstToTarget, obstacleMask))
                        {
                            _targets.Add(collider);
                        }
                    }
                }
                yield return new WaitForSeconds(_updateTime);
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        targetAction = null;
    }
}