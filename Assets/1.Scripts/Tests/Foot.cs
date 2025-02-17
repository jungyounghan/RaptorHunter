using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public sealed class Foot : MonoBehaviour
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

    private bool _hasBoxCollider = false;

    private BoxCollider _boxCollider = null;

    private BoxCollider getBoxCollider
    {
        get
        {
            if(_hasBoxCollider == false)
            {
                _hasBoxCollider = TryGetComponent(out _boxCollider);
            }
            return _boxCollider;
        }
    }

    private List<Collider> _colliders = new List<Collider>();

#if UNITY_EDITOR
    [SerializeField]
    private float _pivotSize = 0.1f;

    [SerializeField]
    private Color _pivotColor = Color.green;

    [SerializeField]
    private Color _contactColor = Color.red;

    private void OnDrawGizmos()
    {
        DrawPoint(getTransform.position, _pivotColor);
    }

    private void DrawPoint(Vector3 point, Color color)
    {
        Debug.DrawLine(point, point + getTransform.up * _pivotSize, color);
        Debug.DrawLine(point, point - getTransform.up * _pivotSize, color);
        Debug.DrawLine(point, point + getTransform.right * _pivotSize, color);
        Debug.DrawLine(point, point - getTransform.right * _pivotSize, color);
        Debug.DrawLine(point, point + getTransform.forward * _pivotSize, color);
        Debug.DrawLine(point, point - getTransform.forward * _pivotSize, color);
    }
#endif

    private void OnCollisionEnter(Collision collision)
    {
        if (_colliders.Contains(collision.collider) == false)
        {
            _colliders.Add(collision.collider);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _colliders.Remove(collision.collider);
    }

    private void LateUpdate()
    {
        List<Vector3> points = new List<Vector3>();
        foreach(Collider collider in _colliders)
        {
            if (collider is MeshCollider meshCollider && meshCollider.convex == false)
            {
#if UNITY_EDITOR
                Debug.Log("메쉬 콜라이더의 컨벡스가 해제 되어 있으면 충돌 검사를 할 수 없습니다.");
#endif
                continue;
            }
            else
            {
                Physics.ComputePenetration(getBoxCollider, getTransform.position, getTransform.rotation, collider, collider.transform.position, collider.transform.rotation, out Vector3 direction, out float distance);
                points.Add(collider.ClosestPoint(getTransform.position - direction * distance));
            }
        }
        foreach(Vector3 point in points)
        {
            DrawPoint(point, _contactColor);
        }
    }
}