using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ǥ���� �� ��ü�� �Ÿ� ���̿� �ִ� �ٸ� ������Ʈ �̹������� �ڽ� ���·� ������ �ʰ� ���ִ� Ŭ����
/// </summary>
[DisallowMultipleComponent]
public sealed class BoxRevealer : MonoBehaviour
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

    [Header("�ٶ� ����� Ʈ������"), SerializeField]
    private Transform _target;
    [Header("�浹 ó���� �� ��ü�� ���̾��ũ"), SerializeField]
    private LayerMask _layerMask;
    [Header("�ٶ� ������ ����"), SerializeField]
    private Vector3 _offset;
    [Header("����� ������ �ڽ��� ũ��"), SerializeField]
    private Vector3 _size;

    private List<Material> _list = new List<Material>();

#if UNITY_EDITOR
    [Header("�浹�� ǥ�� ����"), SerializeField]
    private Color _gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        if (_target != null)
        {
            Vector3 start = getTransform.position;
            Gizmos.color = _gizmoColor;
            Gizmos.matrix = Matrix4x4.TRS(start, Quaternion.LookRotation((_target.position + _offset) - start), Vector3.one);
            Gizmos.DrawWireCube(new Vector3(0, 0, _size.z * 0.5f), _size);
        }
    }

    private void OnValidate()
    {
        if (_size.x < 0)
        {
            _size.x = 0;
        }
        if (_size.y < 0)
        {
            _size.y = 0;
        }
        if (_size.z < 0)
        {
            _size.z = 0;
        }
    }
#endif

    private void Update()
    {
        if (_target != null)
        {
            Vector3 start = getTransform.position;
            Vector3 direction = (_target.position + _offset) - start;
            Vector3 halfSize = _size * 0.5f;
            Vector3 center = start + direction.normalized * halfSize.z;
            Quaternion rotation = Quaternion.LookRotation(direction);
            Matrix4x4 rotMatrix = Matrix4x4.TRS(center, rotation, Vector3.one);
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