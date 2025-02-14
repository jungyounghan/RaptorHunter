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

    [SerializeField]
    private LayerMask _layerMask;

    private List<Material> _list = new List<Material>();

    [SerializeField, Range(0, 360)]
    private float _testAngle = 0;

    [SerializeField]
    private float _testDegree = 0;

#if UNITY_EDITOR
    [Header("충돌선 표시 색깔"), SerializeField]
    private Color _gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawWireSphere(getTransform.position, _radius);
        Vector3 baseDir = transform.right; // 기본 방향 벡터
        float rad = (_testDegree + _testAngle * 0.5f) * Mathf.Deg2Rad; // 각도를 라디안으로 변환

        // Y축 기준으로 회전 적용
        Vector3 rotatedDir = new Vector3(
            baseDir.x * Mathf.Cos(rad) - baseDir.z * Mathf.Sin(rad),
            baseDir.y,
            baseDir.x * Mathf.Sin(rad) + baseDir.z * Mathf.Cos(rad)
        );

        // 결과 출력 (디버깅용)
        Gizmos.DrawRay(getTransform.position, rotatedDir.normalized * _radius);
        rad = (_testDegree - _testAngle * 0.5f) * Mathf.Deg2Rad; // 각도를 라디안으로 변환

        // Y축 기준으로 회전 적용
        rotatedDir = new Vector3(
            baseDir.x * Mathf.Cos(rad) - baseDir.z * Mathf.Sin(rad),
            baseDir.y,
            baseDir.x * Mathf.Sin(rad) + baseDir.z * Mathf.Cos(rad)
        );
        // 결과 출력 (디버깅용)
        Gizmos.DrawRay(getTransform.position, rotatedDir.normalized * _radius);
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
        Vector3 center = getTransform.position;
        Quaternion rotation = getTransform.rotation;
        Matrix4x4 rotMatrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        List<Transform> transforms = new List<Transform>();
        List<Material> materials = new List<Material>();
        Collider[] colliders = Physics.OverlapSphere(center, _radius, _layerMask);
        foreach (Collider collider in colliders)
        {
            Transform transform = collider.transform;
            if (transforms.Contains(transform) == false)
            {
                transforms.Add(transform);
            }
        }
        foreach (Transform transform in transforms)
        {
            MeshRenderer[] meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                foreach (Material material in meshRenderer.materials)
                {
                    if (material.HasFloat(CenterX) && material.HasFloat(CenterY) && material.HasFloat(Degree) && material.HasFloat(Angle))
                    {
                        material.SetFloat(CenterX, center.x);
                        material.SetFloat(CenterY, center.z);
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