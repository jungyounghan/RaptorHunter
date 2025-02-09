using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 목표물과 이 객체의 거리 사이에 있는 다른 오브젝트 이미지들이 보이지 않게 해주는 클래스
/// </summary>
[DisallowMultipleComponent]
public sealed class Revealer : MonoBehaviour
{
    private static readonly string ClippingMin = "_ClipMin";
    private static readonly string ClippingMax = "_ClipMax";
    private static readonly string ClippingCenter = "_ClipCenter";
    private static readonly string RotationRow0 = "_ClipRotRow0";
    private static readonly string RotationRow1 = "_ClipRotRow1";
    private static readonly string RotationRow2 = "_ClipRotRow2";

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

    [Header("바라볼 대상의 트랜스폼"), SerializeField]
    private Transform _target;

    [Header("충돌 처리를 할 물체의 레이어마스크"), SerializeField]
    private LayerMask _layerMask;

    [Header("오프셋 간격"), SerializeField]
    private Vector3 _offset;

    [Header("투과 넓이 영역"), SerializeField, Range(float.Epsilon, 10)]
    private float _width = 2;

    private List<Material> _list = new List<Material>();

#if UNITY_EDITOR
    [Header("충돌선 표시 색깔"), SerializeField]
    private Color _gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        if (_target != null)
        {
            float half = 0.5f;
            Vector3 start = getTransform.position + _offset;
            Vector3 end = _target.position + _offset;
            Vector3 center = (start + end) * half;
            Vector3 direction = end - start;
            Vector3 forward = new Vector3(direction.x, 0, direction.z).normalized;
            float depth = new Vector3(direction.x, 0, direction.z).magnitude;
            Vector3 size = new Vector3(_width, Mathf.Abs(direction.y), depth);
            Quaternion rotation = Quaternion.LookRotation(forward);
            Gizmos.color = _gizmoColor;
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
        }
    }
#endif

    private void Update()
    {
        if (_target != null)
        {
            float half = 0.5f;
            Vector3 start = getTransform.position + _offset;
            Vector3 end = _target.position + _offset;
            Vector3 center = (start + end) * half;
            Vector3 direction = end - start;
            Vector3 forward = new Vector3(direction.x, 0, direction.z).normalized;
            float depth = new Vector3(direction.x, 0, direction.z).magnitude;
            Vector3 halfExtents = new Vector3(_width, Mathf.Abs(direction.y), depth) * half;
            Quaternion rotation = Quaternion.LookRotation(forward);
            Matrix4x4 rotMatrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            List<Material> list = new List<Material>();
            Collider[] colliders = Physics.OverlapBox(center, halfExtents, rotation, _layerMask);
            foreach (Collider collider in colliders)
            {
                MeshRenderer[] meshRenderers = collider.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    foreach (Material material in meshRenderer.materials)
                    {
                        if (material.HasVector(ClippingMin) == true && material.HasVector(ClippingMax) == true && material.HasVector(ClippingCenter) == true &&
                            material.HasVector(RotationRow0) == true && material.HasVector(RotationRow1) == true && material.HasVector(RotationRow2) == true)
                        {
                            material.SetVector(ClippingMin, -halfExtents);
                            material.SetVector(ClippingMax, halfExtents);
                            material.SetVector(ClippingCenter, center);
                            material.SetVector(RotationRow0, rotMatrix.GetRow(0));
                            material.SetVector(RotationRow1, rotMatrix.GetRow(1));
                            material.SetVector(RotationRow2, rotMatrix.GetRow(2));
                            list.Add(material);
                        }
                    }
                }
            }
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (list.Contains(_list[i]) == false)
                {
                    Material material = _list[i];
                    material.SetVector(ClippingMin, new Vector4(0, 0, 0, 0));
                    material.SetVector(ClippingMax, new Vector4(1, 1, 1, 0));
                    material.SetVector(ClippingCenter, new Vector4(0, 0, 0, 0));
                    material.SetVector(RotationRow0, new Vector4(1, 0, 0, 0));
                    material.SetVector(RotationRow1, new Vector4(0, 1, 0, 0));
                    material.SetVector(RotationRow2, new Vector4(0, 0, 1, 0));
                    _list.Remove(material);
                }
            }
            foreach (Material material in list)
            {
                if (_list.Contains(material) == false)
                {
                    _list.Add(material);
                }
            }
        }
    }
}