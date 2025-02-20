using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("시야의 초점각"), SerializeField]
    private float _focusAngle = 0;

    [Header("시야의 범위각"), SerializeField, Range(0, 360)]
    private float _rangeAngle = 60;

    [Header("감지 갱신 간격"), SerializeField, Range(0, byte.MaxValue)]
    private float _updateTime = 0.4f;

    [Header("감지 영역의 반경"), SerializeField, Range(0, int.MaxValue)]
    private float _radius = 50;

    [Header("중점 오프셋"), SerializeField]
    private Vector3 _offset;

    [Header("수색 대상 마스크"), SerializeField]
    private LayerMask _layerMask;

    public event Action<IEnumerable<Collider>> collidersAction = null;

#if UNITY_EDITOR
    [Header("범위 표시 색깔"), SerializeField]
    private Color _rangeColor = Color.white;
    [Header("포착 표시 색깔"), SerializeField]
    private Color _targetColor = Color.red;

    private void OnDrawGizmos()
    {
        float focusDegree = getTransform.eulerAngles.y + _focusAngle - 90;
        float angleHalf = _rangeAngle * 0.5f;
        float positiveRadian = (focusDegree + angleHalf) * Mathf.Deg2Rad;
        float negativeRadian = (focusDegree - angleHalf) * Mathf.Deg2Rad;
        Vector3 position = getTransform.position + _offset;
        Gizmos.color = _rangeColor;
        Gizmos.DrawWireSphere(position, _radius);
        Gizmos.DrawLine(new Vector3(position.x + _radius, position.y, position.z), new Vector3(position.x - _radius, position.y, position.z));
        Gizmos.DrawLine(new Vector3(position.x, position.y + _radius, position.z), new Vector3(position.x, position.y - _radius, position.z));
        Gizmos.DrawLine(new Vector3(position.x, position.y, position.z + _radius), new Vector3(position.x, position.y, position.z - _radius));
        Gizmos.DrawLine(position, position + new Vector3(Mathf.Cos(positiveRadian), 0, Mathf.Sin(positiveRadian)).normalized * _radius);
        Gizmos.DrawLine(position, position + new Vector3(Mathf.Cos(negativeRadian), 0, Mathf.Sin(negativeRadian)).normalized * _radius);
    }
#endif

    private void OnEnable()
    {
        StartCoroutine(DoDetectTargets());
        IEnumerator DoDetectTargets()
        {
            while (true)
            {
                float focusDegree = getTransform.eulerAngles.y + _focusAngle - 90;
                float radian = focusDegree * Mathf.Deg2Rad;
                Vector3 center = getTransform.position + _offset;
                Vector3 currentAngle = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian));
                List<Collider> list = new List<Collider>();
                Collider[] colliders = Physics.OverlapSphere(center, _radius, _layerMask);
                foreach (Collider collider in colliders)
                {
                    Vector3 position = collider.bounds.center;
                    Vector3 direction = (position - center).normalized;
                    if (Vector3.Angle(currentAngle, direction) < _rangeAngle * 0.5f && collider.isTrigger == false &&  getTransform != collider.transform)
                    {
                        if (Physics.Raycast(new Ray(center, direction), out RaycastHit raycastHit, Vector3.Distance(center, position), _layerMask) == false)
                        {
                            list.Add(collider);
                        }
                        else if (raycastHit.collider.isTrigger == false && list.Contains(raycastHit.collider) == false)
                        {
                            list.Add(raycastHit.collider);
                        }
                    }
                }

#if UNITY_EDITOR
                foreach (Collider collider in list)
                {
                    Debug.DrawLine(center, collider.bounds.center, _targetColor, _updateTime);
                }
#endif
                collidersAction?.Invoke(list);
                list.Clear();
                yield return new WaitForSeconds(_updateTime);
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        collidersAction = null;
    }
}