using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 목표물과 이 객체의 거리 사이에 있는 다른 오브젝트 이미지들이 보이지 않게 해주는 클래스
/// </summary>
[DisallowMultipleComponent]
public sealed class Revealer : MonoBehaviour
{
    private static readonly string ClippingMin = "_ClippingMin";
    private static readonly string ClippingMax = "_ClippingMax";
    private static readonly string ClippingCenter = "_ClippingCenter";
    private static readonly string RotationRow0 = "_RotationRow0";
    private static readonly string RotationRow1 = "_RotationRow1";
    private static readonly string RotationRow2 = "_RotationRow2";

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

    private List<MeshRenderer> _list = new List<MeshRenderer>();

#if UNITY_EDITOR
    [Header("충돌선 표시 색깔"), SerializeField]
    private Color _gizmoColor = Color.blue;

    private void OnDrawGizmos()
    {
        if (_target != null)
        {
            Vector3 start = getTransform.position;
            Vector3 end = _target.position + _offset;
            Gizmos.color = _gizmoColor;
            Gizmos.matrix = Matrix4x4.TRS((start + end) * 0.5f, Quaternion.LookRotation(end - start), Vector3.one);
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
            Vector3 halfSize = new Vector3(_width * 0.5f, _height * 0.5f, Vector3.Distance(start, end) * 0.5f);
            Quaternion rotation = Quaternion.LookRotation(end - start);
            List<MeshRenderer> list = new List<MeshRenderer>();
            Collider[] colliders = Physics.OverlapBox(center, halfSize, rotation, _layerMask);
            foreach (Collider collider in colliders)
            {
                MeshRenderer[] meshRenderers = collider.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    bool find = false;
                    Transform parent = meshRenderer.transform.parent;
                    while(parent != null)
                    {
                        if(parent == _target)
                        {
                            find = true;
                            break;
                        }
                        parent = parent.parent;
                    }
                    if (find == true)
                    {
                        continue;
                    }
                    meshRenderer.enabled = false;
                    list.Add(meshRenderer);
                }
            }
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if(list.Contains(_list[i]) == false)
                {
                    MeshRenderer meshRenderer = _list[i];
                    meshRenderer.enabled = true;
                    _list.Remove(meshRenderer);
                }
            }
            foreach(MeshRenderer meshRenderer in list)
            {
                if(_list.Contains(meshRenderer) == false)
                {
                    _list.Add(meshRenderer);
                }
            }
        }
    }
}