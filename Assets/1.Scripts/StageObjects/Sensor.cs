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

    [Header("시야 영역의 초점각"), SerializeField]
    public float _focusAngle;
    [Header("시야 영역의 범위각"), SerializeField, Range(0, 360)]
    public float _rangeAngle;
    [Header("시야 갱신 간격"), SerializeField, Range(0, byte.MaxValue)]
    private float _updateTime = 0.4f;
    [Header("시야 영역의 반경"), SerializeField, Range(0, int.MaxValue)]
    public float _radius;
    [Header("시야 영역의 오프셋"), SerializeField]
    private Vector3 _offset;

    public LayerMask _layerMask;

    public LayerMask obstacleMask;

    public List<Collider> _targets = new List<Collider>();

    public event Action<IEnumerable<Transform>> targetAction = null;

#if UNITY_EDITOR
    [Header("범위 표시 색깔"), SerializeField]
    private Color _rangeColor = Color.white;
    [Header("포착 표시 색깔"), SerializeField]
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
                    // 플레이어와 forward와 target이 이루는 각이 설정한 각도 내라면
                    if (Vector3.Angle(/*transform.forward*/currentAngle, direction) < _rangeAngle * 0.5f)
                    {
                        float dstToTarget = Vector3.Distance(center, position);

                        // 타겟으로 가는 레이캐스트에 obstacleMask가 걸리지 않으면 visibleTargets에 Add
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