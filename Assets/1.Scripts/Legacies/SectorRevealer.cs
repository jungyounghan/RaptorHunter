using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 목표물과 이 객체의 거리 사이에 있는 다른 오브젝트 이미지들을 부채꼴 형태로 보이지 않게 해주는 클래스
/// </summary>
[DisallowMultipleComponent]
public sealed class SectorRevealer : MonoBehaviour
{
    private static readonly string CenterX = "_CenterX";
    private static readonly string CenterY = "_CenterY";
    private static readonly string Degree = "_Degree";
    private static readonly string Angle = "_Angle";

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

    private List<Material> _list = new List<Material>();

    [Header("바라볼 대상의 트랜스폼"), SerializeField]
    private Transform _target;

    [SerializeField]
    private LayerMask _layerMask;

#if UNITY_EDITOR

    private Color _targetColor = Color.black;

    private Color _leftColor = Color.red;

    private Color _rightColor = Color.blue;

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false && _target != null)
        {
            Vector3 center = getTransform.position;
            Vector3 target = _target.position;
            target.y = center.y;
            Vector3 direction = target - center;
            Debug.DrawRay(center, direction, _targetColor);
            Vector3 cross = Vector3.Cross(direction, Vector3.up);
            Debug.DrawRay(target, (center - cross) - target, _leftColor);
            Debug.DrawRay(target, (center + cross) - target, _rightColor);
        }
    }
#endif

    private void Update()
    {
        if (_target != null)
        {
            Vector3 center = getTransform.position;
            Vector3 target = _target.position;
            target.y = center.y;
            Vector3 direction = target - center;
#if UNITY_EDITOR
            Debug.DrawRay(center, direction, _targetColor);
#endif
            Vector3 cross = Vector3.Cross(direction, Vector3.up);
            float distance = Mathf.Sqrt(Mathf.Pow(direction.magnitude, 2) * 2);
            Vector3 left = center - cross;
            Vector3 right = center + cross;
            if (Physics.Raycast(target, (center - cross) - target, out RaycastHit leftHit, distance, _layerMask))
            {
                left = leftHit.point;
            }
            if (Physics.Raycast(target, (center + cross) - target, out RaycastHit rightHit, distance, _layerMask))
            {
                right = rightHit.point;
            }
#if UNITY_EDITOR
            Debug.DrawLine(target, left, _leftColor);
            Debug.DrawLine(target, right, _rightColor);
#endif
            Vector2 o = new Vector2(center.x, center.z);
            Vector2 ao = new Vector2(left.x, left.z) - o;
            Vector2 bo = new Vector2(right.x, right.z) - o;
            float denominator = ao.magnitude * bo.magnitude;
            float angle = Mathf.Acos(Mathf.Clamp(Vector2.Dot(ao, bo) / denominator, -1f, 1f)) * Mathf.Rad2Deg;
            float degree = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            if (angle < 180)
            {
                Vector3 a = Vector3.Cross(direction, -Vector3.up);
                Vector3 b = Vector3.Cross(direction, Vector3.up);
                degree += Vector2.Angle(ao, new Vector2(a.x, a.z)) * 0.5f;
                degree -= Vector2.Angle(bo, new Vector2(b.x, b.z)) * 0.5f;
            }
            foreach (Material material in _list)
            {
                material.SetFloat(CenterX, center.x);
                material.SetFloat(CenterY, center.z);
                material.SetFloat(Degree, degree);
                material.SetFloat(Angle, angle);
            }
        }
        else
        {
            foreach (Material material in _list)
            {
                material.SetFloat(Angle, 360);
            }
        }
    }

    public void Add(IEnumerable<Material> materials)
    {
        if (materials != null)
        {
            foreach (Material material in materials)
            {
                if (_list.Contains(material) == false && material.HasFloat(CenterX) && material.HasFloat(CenterY) && material.HasFloat(Degree) && material.HasFloat(Angle))
                {
                    _list.Add(material);
                }
            }
        }
    }

    public void Remove(IEnumerable<Material> materials)
    {
        if (materials != null)
        {
            foreach (Material material in materials)
            {
                if (_list.Contains(material) == true)
                {
                    material.SetFloat(Angle, 360);
                    _list.Remove(material);
                }
            }
        }
    }
}