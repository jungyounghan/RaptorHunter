using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 목표물과 이 객체의 거리 사이에 있는 다른 오브젝트 이미지들을 부채꼴 형태로 보이지 않게 해주는 클래스
/// </summary>
[DisallowMultipleComponent]
public sealed class SectorPenetrator : MonoBehaviour
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

    [SerializeField]
    private float _radius = 100;
    public float radius
    {
        get
        {
            return _radius;
        }
        set
        {
            _radius = value;
        }
    }

    [Header("위치 오프셋"), SerializeField]
    private Vector3 _offset;

    [Header("레이어 마스크"), SerializeField]
    private LayerMask _layerMask;

    private List<Material> _list = new List<Material>();

    [SerializeField, Range(0, 360)]
    private float _testAngle = 0;

    [SerializeField]
    private float _testDegree = 0;

#if UNITY_EDITOR
    [Header("사각형 표시 색깔"), SerializeField]
    private Color _boxColor = Color.green;
    [Header("중앙 표시 색깔"), SerializeField]
    private Color _centerColor = Color.black;
    [Header("중앙 표시 색깔"), SerializeField]
    private Color _leftColor = Color.red;
    [Header("중앙 표시 색깔"), SerializeField]
    private Color _rightColor = Color.blue;

    private void OnDrawGizmos()
    {
        Vector3 target = getTransform.position;
        Gizmos.color = _boxColor;
        Gizmos.DrawWireSphere(target, _radius);
        if (Application.isPlaying == false)
        {
            target.y += _offset.y;
            Vector3 pivot = target - getTransform.forward * _offset.z;
            Vector3 direction = pivot - target;
            Vector3 cross = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 left = pivot - cross * _offset.x * 0.5f;
            Vector3 right = pivot + cross * _offset.x * 0.5f;
            Gizmos.color = _centerColor;
            Gizmos.DrawLine(target, pivot);
            Gizmos.color = _leftColor;
            Gizmos.DrawLine(pivot, left);
            Gizmos.DrawRay(target, (left - target));
            Gizmos.color = _rightColor;
            Gizmos.DrawLine(pivot, right);
            Gizmos.DrawRay(target, (right - target));
        }
    }
#endif

    private void OnDisable()
    {
        for (int i = _list.Count - 1; i >= 0; i--)
        {
            Material material = _list[i];
            material.SetFloat(Angle, 0);
            _list.Remove(material);
        }
    }

    private void LateUpdate()
    {
        Vector3 target = getTransform.position;
        List<Transform> transforms = new List<Transform>();
        List<Material> materials = new List<Material>();
        Collider[] colliders = Physics.OverlapSphere(target, _radius, _layerMask);
        foreach (Collider collider in colliders)
        {
            Transform transform = collider.transform;
            if (transforms.Contains(transform) == false)
            {
                transforms.Add(transform);
            }
        }
        target.y += _offset.y;
        Vector3 pivot = target - getTransform.forward * _offset.z;
        Vector3 direction = pivot - target;
        Vector3 cross = Vector3.Cross(direction, Vector3.up).normalized;
        float distance = Mathf.Sqrt(Mathf.Pow(direction.magnitude, 2) + Mathf.Pow(cross.magnitude * _offset.x * 0.5f, 2));
        Vector3 left = pivot - cross * _offset.x * 0.5f;
        Vector3 right = pivot + cross * _offset.x * 0.5f;
        if (Physics.Raycast(target, (left - target), out RaycastHit leftHit, distance, _layerMask))
        {
            left = leftHit.point;
        }
        if (Physics.Raycast(target, (right - target), out RaycastHit rightHit, distance, _layerMask))
        {
            right = rightHit.point;
        }   
#if UNITY_EDITOR
        Debug.DrawLine(target, pivot, _centerColor);
        Debug.DrawLine(pivot, left, _leftColor);
        Debug.DrawRay(target, (left - target).normalized * distance, _leftColor);
        Debug.DrawLine(pivot, right, _rightColor);
        Debug.DrawRay(target, (right - target).normalized * distance, _rightColor);
#endif
        Vector2 o = new Vector2(pivot.x, pivot.z);
        Vector2 ao = new Vector2(left.x, left.z) - o;
        Vector2 bo = new Vector2(right.x, right.z) - o;
        float denominator = ao.magnitude * bo.magnitude;
        float angle = Mathf.Acos(Mathf.Clamp(Vector2.Dot(ao, bo) / denominator, -1f, 1f)) * Mathf.Rad2Deg;
        float degree = Mathf.Atan2(-direction.z, -direction.x) * Mathf.Rad2Deg;
        if (angle < 180)
        {
            Vector3 a = Vector3.Cross(direction, -Vector3.up);
            Vector3 b = Vector3.Cross(direction, Vector3.up);
            degree -= Vector2.Angle(ao, new Vector2(a.x, a.z)) * 0.5f;
            degree += Vector2.Angle(bo, new Vector2(b.x, b.z)) * 0.5f;
        }
        _testAngle = angle;
        _testDegree = degree;
        foreach (Transform transform in transforms)
        {
            MeshRenderer[] meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                foreach (Material material in meshRenderer.materials)
                {
                    if (material.HasFloat(CenterX) && material.HasFloat(CenterY) && material.HasFloat(Degree) && material.HasFloat(Angle))
                    {
                        material.SetFloat(CenterX, pivot.x);
                        material.SetFloat(CenterY, pivot.z);
                        material.SetFloat(Degree, _testDegree);
                        material.SetFloat(Angle, _testAngle);
                        materials.Add(material);
                    }
                }
            }
        }
        for (int i = _list.Count - 1; i >= 0; i--)
        {
            if (materials.Contains(_list[i]) == false)
            {
                Material material = _list[i];
                material.SetFloat(Angle, 360);
                _list.Remove(material);
            }
        }
        foreach (Material material in materials)
        {
            if (_list.Contains(material) == false)
            {
                _list.Add(material);
            }
        }
    }

}