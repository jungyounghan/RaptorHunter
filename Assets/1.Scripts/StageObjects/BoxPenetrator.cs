using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 목표물과 이 객체의 거리 사이에 있는 다른 오브젝트 이미지들을 박스 형태로 보이지 않게 해주는 클래스
/// </summary>
[DisallowMultipleComponent]
public sealed class BoxPenetrator : MonoBehaviour
{
    private static readonly string ClippingMin = "_ClipMin";
    private static readonly string ClippingMax = "_ClipMax";
    private static readonly string ClippingCenter = "_ClipCenter";
    private static readonly string RotationRow0 = "_ClipRotRow0";
    private static readonly string RotationRow1 = "_ClipRotRow1";
    private static readonly string RotationRow2 = "_ClipRotRow2";

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
    private Vector3 _size = Vector3.one;
    public Vector3 size
    {
        get
        {
            return _size;
        }
        set
        {
            _size = value;
        }
    }

    [SerializeField]
    private LayerMask _layerMask;

    private List<Material> _list = new List<Material>();


#if UNITY_EDITOR
    [Header("충돌선 표시 색깔"), SerializeField]
    private Color _gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(getTransform.position, getTransform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, _size);
    }
#endif

    private void OnDisable()
    {
        for (int i = _list.Count - 1; i >= 0; i--)
        {
            Material material = _list[i];
            material.SetVector(ClippingMin, new Vector4(0, 0, 0, 0));
            material.SetVector(ClippingMax, new Vector4(0, 0, 0, 0));
            _list.Remove(material);
        }
    }

    private void LateUpdate()
    {
        Vector3 center = getTransform.position;
        Vector3 halfSize = _size * 0.5f;
        Quaternion rotation = getTransform.rotation;
        Matrix4x4 rotMatrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        List<Transform> transforms = new List<Transform>();
        List<Material> materials = new List<Material>();
        Collider[] colliders = Physics.OverlapBox(center, halfSize, rotation, _layerMask);
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
                    if (material.HasVector(ClippingMin) == true && material.HasVector(ClippingMax) == true && material.HasVector(ClippingCenter) == true &&
                        material.HasVector(RotationRow0) == true && material.HasVector(RotationRow1) == true && material.HasVector(RotationRow2) == true)
                    {
                        material.SetVector(ClippingMin, -halfSize);
                        material.SetVector(ClippingMax, halfSize);
                        material.SetVector(ClippingCenter, center);
                        material.SetVector(RotationRow0, rotMatrix.GetRow(0));
                        material.SetVector(RotationRow1, rotMatrix.GetRow(1));
                        material.SetVector(RotationRow2, rotMatrix.GetRow(2));
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
                material.SetVector(ClippingMin, new Vector4(0, 0, 0, 0));
                material.SetVector(ClippingMax, new Vector4(0, 0, 0, 0));
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