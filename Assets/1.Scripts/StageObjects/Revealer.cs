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
    [Header("바라볼 대상과의 간격"), SerializeField]
    private Vector3 _offset;
    [Header("가로 크기"), SerializeField, Range(0, byte.MaxValue)]
    private float _width = 2;
    [Header("세로 크기"), SerializeField, Range(0, byte.MaxValue)]
    private float _height = 2;
    [Header("블러 부드러움"), SerializeField, Range(0, byte.MaxValue)]
    private float _blurSoftness = 0.2f;
    [Header("블러 강도 조절"), SerializeField, Range(0, byte.MaxValue)]
    private float _blurStrength = 5;


    private List<Material> _list = new List<Material>();

#if UNITY_EDITOR
    [Header("충돌선 표시 색깔"), SerializeField]
    private Color _gizmoColor = Color.blue;

    private void OnDrawGizmos()
    {
        if (_target != null)
        {
            Vector3 start = getTransform.position;
            Vector3 end = _target.position + _offset;
            Quaternion rotation = Quaternion.LookRotation(end - start);
            Gizmos.color = _gizmoColor;
            Gizmos.matrix = Matrix4x4.TRS((start + end) * 0.5f, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(_width, _height, Vector3.Distance(start, end)));
        }
    }


#endif

    private void Update()
    {
        if (_target != null)
        {
            Vector3 start = getTransform.position;
            Vector3 end = _target.position + _offset;
            Vector3 center = (start + end) * 0.5f;
            Quaternion rotation = Quaternion.LookRotation(end - start);
            Vector3 halfSize = new Vector3(_width * 0.5f, _height * 0.5f, Vector3.Distance(start, end) * 0.5f);
            Matrix4x4 rotMatrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
            List<Material> list = new List<Material>();
            Collider[] colliders = Physics.OverlapBox(center, halfSize, rotation, _layerMask);
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
                            material.SetVector(ClippingMin, -halfSize);
                            material.SetVector(ClippingMax, halfSize);
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
                if(list.Contains(_list[i]) == false)
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
            foreach(Material material in list)
            {
                if(_list.Contains(material) == false)
                {
                    _list.Add(material);
                }
            }
        }
    }
}