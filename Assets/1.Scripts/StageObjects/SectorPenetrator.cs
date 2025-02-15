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
    private float _radius = 0;
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
    private float _offset;

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

    [Header("각도 표시 색깔"), SerializeField]
    private Color _angleColor = Color.red;

    private void OnDrawGizmos()
    {
        Gizmos.color = _boxColor;
        Vector3 position = getTransform.position;
        Gizmos.DrawWireSphere(position, _radius);
        Gizmos.color = _angleColor;
        Vector3 top = position + new Vector3(0, _radius, 0);
        Vector3 bottom = position - new Vector3(0, _radius, 0);
        Gizmos.DrawLine(top, bottom);
        for (float i = _testDegree; i < _testDegree + _testAngle; i += 2)
        {
            DrawSquareBracketShape(top, bottom, (_testDegree + i) * 0.5f * Mathf.Deg2Rad);
            DrawSquareBracketShape(top, bottom, (_testDegree + i - _testAngle) * 0.5f * Mathf.Deg2Rad);
        }
    }

    private void DrawSquareBracketShape(Vector3 top, Vector3 bottom, float radian)
    {
        Vector3 direction = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian));
        //float size = Mathf.Min((direction.x != 0) ? (direction.x > 0 ? _radius : -_radius) / direction.x : _radius, (direction.z != 0) ? (direction.z > 0 ? _radius : -_radius) / direction.z : _radius);
        Vector3 line = direction.normalized * _radius/* size*/; //원래는 _radius 대신 size를 썼었다.
        Gizmos.DrawRay(top, line);
        Gizmos.DrawRay(bottom, line);
        Gizmos.DrawLine(top + line, bottom + line);
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
        Vector3 pivot = getTransform.position;
        List<Transform> transforms = new List<Transform>();
        List<Material> materials = new List<Material>();
        Collider[] colliders = Physics.OverlapSphere(pivot, _radius, _layerMask);
        foreach (Collider collider in colliders)
        {
            Transform transform = collider.transform;
            if (transforms.Contains(transform) == false)
            {
                transforms.Add(transform);
            }
        }
        Vector3 center = pivot - (getTransform.forward.normalized * _offset);

#if UNITY_EDITOR
        Debug.DrawLine(pivot, center);
#endif

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